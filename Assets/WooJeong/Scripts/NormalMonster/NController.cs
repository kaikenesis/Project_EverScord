using EverScord;
using EverScord.Character;
using EverScord.Effects;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public enum MonsterType
{
    SMALL, MEDIUM, LARGE
}

public abstract class NController : MonoBehaviour, IEnemy
{
    [SerializeField] public NMonsterData monsterData;

    public float HP {  get; private set; }

    [HideInInspector] public Dictionary<string, float> clipDict = new();
    [HideInInspector] public int LastAttack = 0;
    [HideInInspector] public GameObject player;
    [HideInInspector] public float stunTime = 2;
    [HideInInspector] public bool isStun = false;
    [HideInInspector] public bool isDead = false;
    protected float curCool1 = 0;
    protected float curCool2 = 0;
    protected RaycastHit hit;
    protected LayerMask playerLayer;
    protected MonsterType monsterType;

    public DecalProjector Projector1 { get; protected set; }
    public DecalProjector Projector2 { get; protected set; }
    public BoxCollider BoxCollider1 { get; protected set; }
    public BoxCollider BoxCollider2 { get; protected set; }
    public Animator Animator { get; protected set; }
    public BoxCollider HitBox { get; protected set; }

    public string GUID { get; protected set; }

    private BlinkEffect blinkEffect;
    private UIMarker uiMarker;

    public PhotonView PhotonView => photonView;
    private PhotonView photonView;
    protected MonsterHealthBar monsterHealthBar;
    protected GameObject healthBarObject;

    protected IState currentState;
    protected IState runState;
    protected IState attackState1;
    protected IState attackState2;
    protected IState waitState;
    protected IState stunState;
    protected IState deathState;

    protected abstract void Setup();

    protected void Awake()
    {
        photonView = GetComponent<PhotonView>();
        Animator = GetComponentInChildren<Animator>();
        HitBox = GetComponent<BoxCollider>();
        Projector1 = gameObject.AddComponent<DecalProjector>();
        Projector2 = gameObject.AddComponent<DecalProjector>();
        BoxCollider1 = gameObject.AddComponent<BoxCollider>();
        BoxCollider2 = gameObject.AddComponent<BoxCollider>();
        uiMarker = gameObject.AddComponent<UIMarker>();
        
        uiMarker.Initialize(PointMarkData.EType.Monster);
        ProjectorSetup();

        blinkEffect = BlinkEffect.Create(this);

        Projector1.enabled = false;
        Projector2.enabled = false;
        BoxCollider1.isTrigger = true;
        BoxCollider2.isTrigger = true;
        BoxCollider1.enabled = false;
        BoxCollider2.enabled = false;

        foreach (AnimationClip clip in Animator.runtimeAnimatorController.animationClips)
        {
            clipDict[clip.name] = clip.length;
        }

        Setup();

        if (!PhotonNetwork.IsMasterClient)
            return;

        playerLayer = LayerMask.GetMask("Player");
        SetNearestPlayer();
    }
    protected void ProjectorSetup()
    {
        Projector1.renderingLayerMask = 2;
        Projector1.material = ResourceManager.Instance.GetAsset<Material>("DecalRedSquare");
        Projector2.renderingLayerMask = 2;
        Projector2.material = ResourceManager.Instance.GetAsset<Material>("DecalRedSquare");
        Projector1.size = new Vector3(monsterData.AttackRangeX1,
                                      monsterData.AttackRangeY1,
                                      monsterData.AttackRangeZ1);

        Projector1.pivot = new Vector3(0, transform.position.y,
                                       monsterData.AttackRangeZ1 / 2);

        Projector2.size = new Vector3(monsterData.AttackRangeX2,
                                      monsterData.AttackRangeY2,
                                      monsterData.AttackRangeZ2);

        Projector2.pivot = new Vector3(0, transform.position.y,
                                       monsterData.AttackRangeZ2 / 2);

        BoxCollider1.center = new Vector3(0, transform.position.y,
                                          monsterData.AttackRangeZ1 / 2);

        BoxCollider1.size = new Vector3(monsterData.AttackRangeX1,
                                        monsterData.AttackRangeY1,
                                        monsterData.AttackRangeZ1);

        BoxCollider2.center = new Vector3(0, transform.position.y,
                                        monsterData.AttackRangeZ2 / 2);

        BoxCollider2.size = new Vector3(monsterData.AttackRangeX2,
                                        monsterData.AttackRangeY2,
                                        monsterData.AttackRangeZ2);
    }

    private void Start()
    {
        //SetHealthBar();
        LevelControl.OnProgressUpdated += ProgressCheck;
    }

    private void OnEnable()
    {
        uiMarker.SetActivate(true);
    }

    private void OnDisable()
    {
        uiMarker.SetActivate(false);
    }

    private void Update()
    {
        uiMarker.UpdatePosition(transform.position);
    }

    protected void ProgressCheck(float currentProgress)
    {
        if (currentProgress == 1)
        {
            // 현재 진행도 체크하고 다 됐으면 죽임
            isDead = true;
        }
    }

    protected virtual void SetHealthBar()
    {
        if (healthBarObject != null)
            return;

        // 풀에서 체력바 얻기
        healthBarObject = ResourceManager.Instance.GetFromPool("MonsterHealthBar", Vector3.zero, Quaternion.identity);
        Transform canvas = GameObject.FindGameObjectWithTag("MonsterUI").transform;
        healthBarObject.transform.SetParent(canvas);

        monsterHealthBar = healthBarObject.GetComponent<MonsterHealthBar>();
        monsterHealthBar.SetTarget(transform);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        if (other.gameObject.CompareTag("Player"))
        {
            CharacterControl controller = other.GetComponent<CharacterControl>();
            if (LastAttack == 1)
                Debug.Log("Attack1");
            else
                Debug.Log("Attack2");

            controller.DecreaseHP(10);
        }
    }

    public void SetGUID(string guid)
    {
        GUID = guid;
    }

    public void InstantiateMonsterAttack(Vector3 pos, float width, float projectTime, string addressableKey, float attackDamage)
    {
        photonView.RPC("SyncMonsterAttack", RpcTarget.All, pos, width, projectTime, addressableKey, attackDamage);
    }

    [PunRPC]
    protected void SyncMonsterAttack(Vector3 pos, float width, float projectTime, string addressableKey, float attackDamage)
    {
        GameObject go = ResourceManager.Instance.GetFromPool("MonsterAttack", pos, Quaternion.identity);
        MonsterAttack ma = go.GetComponent<MonsterAttack>();
        ma.Setup(width, projectTime, addressableKey, attackDamage);
    }

    public void DecreaseHP(float hp)
    {
        this.HP -= hp;
        if (this.HP <= 0)
            isDead = true;
        monsterHealthBar.UpdateHealth(this.HP, monsterData.HP);
        photonView.RPC("SyncMonsterHP", RpcTarget.Others, this.HP);
    }

    [PunRPC]
    protected void SyncMonsterHP(float hp)
    {
        this.HP = hp;
        if (this.HP <= 0)
            isDead = true;
        monsterHealthBar.UpdateHealth(this.HP, monsterData.HP);
    }

    public void StunMonster(float stunTime)
    {
        photonView.RPC("SyncMonsterStun", RpcTarget.All, stunTime);
    }

    [PunRPC]
    protected void SyncMonsterStun(float stunTime)
    {
        isStun = true;
        this.stunTime = stunTime;
    }

    public void Death()
    {
        photonView.RPC("SyncMonsterDeath", RpcTarget.Others);
        healthBarObject.SetActive(false);
        GameManager.Instance.LevelController.IncreaseProgress(monsterType);
        ResourceManager.Instance.ReturnToPool(gameObject, GUID);
        //ResourceManager.Instance.ReturnToPool(healthBarObject, "MonsterHealthBar");
        //healthBarObject = null;
    }

    [PunRPC]
    protected void SyncMonsterDeath()
    {
        healthBarObject.SetActive(false);
        Debug.Log(monsterType);
        GameManager.Instance.LevelController.IncreaseProgress(monsterType);
        ResourceManager.Instance.ReturnToPool(gameObject, GUID);
        //ResourceManager.Instance.ReturnToPool(healthBarObject, "MonsterHealthBar");
        //healthBarObject = null;
    }

    public void StartFSM()
    {
        if (healthBarObject == null)
        {
            SetHealthBar();
            healthBarObject.SetActive(true);
            photonView.RPC("SyncSetHealthBar", RpcTarget.Others);
        }
        healthBarObject.SetActive(true);
        photonView.RPC("SyncHealthBarActive", RpcTarget.Others, true);

        if (PhotonNetwork.IsMasterClient)
        {
            isDead = false;
            HP = monsterData.HP;
            monsterHealthBar.UpdateHealth(HP, monsterData.HP);
            photonView.RPC("SyncMonsterHP", RpcTarget.Others, HP);
            LastAttack = 0;
            WaitState();
        }
    }
    [PunRPC]
    protected void SyncSetHealthBar()
    {
        SetHealthBar();
        healthBarObject.SetActive(true);
        HitBox.enabled = true;
    }

    [PunRPC]
    protected void SyncHealthBarActive(bool value)
    {
        healthBarObject.SetActive(true);
        HitBox.enabled = value;
    }

    public void PlayAnimation(string animationName)
    {
        Animator.CrossFade(animationName, 0.3f, -1, 0);
        photonView.RPC("SyncAnimation", RpcTarget.Others, animationName);
    }

    [PunRPC]
    protected void SyncAnimation(string animationName)
    {
        Animator.CrossFade(animationName, 0.3f, -1, 0);
    }

    public virtual IEnumerator ProjectAttackRange(int attackNum)
    {
        DecalProjector projector;
        if (attackNum == 1)
            projector = Projector1;
        else
            projector = Projector2;

        photonView.RPC("SyncProjectorEnable", RpcTarget.Others, attackNum);
        projector.enabled = true;
        yield return new WaitForSeconds(monsterData.ProjectionTime);
        photonView.RPC("SyncProjectorDisable", RpcTarget.Others, attackNum);
        projector.enabled = false;
    }

    public void ProjectorDisable(int projectorNum)
    {
        if (projectorNum == 1)
            Projector1.enabled = false;
        else
            Projector2.enabled = false;
        photonView.RPC("SyncProjectorDisable", RpcTarget.Others, projectorNum);
    }

    [PunRPC]
    protected void SyncProjectorEnable(int projectorNum)
    {
        if (projectorNum == 1)
            Projector1.enabled = true;
        else
            Projector2.enabled = true;
    }
    
    [PunRPC]
    protected void SyncProjectorDisable(int projectorNum)
    {
        if (projectorNum == 1)
            Projector1.enabled = false;
        else
            Projector2.enabled = false;
    }
    public void Fire(string projectileName)
    {
        Vector3 position = transform.position + transform.forward * 2;
        float projectileSpeed = 20;
        GameObject go = ResourceManager.Instance.GetFromPool("MonsterProjectile", position, Quaternion.identity);
        MonsterProjectile mp = go.GetComponent<MonsterProjectile>();

        int id;
        if (mp.ID == -1)
        {
            id = GameManager.Instance.ProjectileController.GetIDNum();
            GameManager.Instance.ProjectileController.AddDict(id, mp);
        }
        else
            id = mp.ID;

        mp.Setup(projectileName, id, position, transform.forward, projectileSpeed);
        photonView.RPC("SyncProjectileNMM2", RpcTarget.Others, projectileName, id, position, transform.forward, projectileSpeed);
    }

    [PunRPC]
    protected void SyncProjectileNMM2(string projectileName, int id, Vector3 position, Vector3 direction, float projectileSpeed)
    {
        GameObject go = ResourceManager.Instance.GetFromPool("MonsterProjectile", position, Quaternion.identity);
        MonsterProjectile mp = go.GetComponent<MonsterProjectile>();
        GameManager.Instance.ProjectileController.AddDict(id, mp);
        mp.Setup(projectileName, id, position, transform.forward, projectileSpeed);
    }

    public void SetNearestPlayer()
    {
        float nearest = Mathf.Infinity;
        GameObject nearPlayer = null;

        foreach (var kv in GameManager.Instance.PlayerDict)
        {
            CharacterControl player = kv.Value;

            if(player.IsDead)
                continue;

            float cur = (player.PlayerTransform.position - transform.position).magnitude;
            if (cur < nearest)
            {
                nearest = cur;
                nearPlayer = player.gameObject;
            }
        }

        if (nearPlayer != null)
            player = nearPlayer;
        //else
            //isDead = true;
    }

    public void LookPlayer()
    {
        if (player != null)
        {
            Vector3 dir = player.transform.position - transform.position;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * monsterData.SmoothAngleSpeed);
            transform.rotation = new(0, transform.rotation.y, 0, transform.rotation.w);
        }
    }

    public bool IsLookPlayer(float distance)
    {
        Vector3 start = new(transform.position.x, transform.position.y + 0.3f, transform.position.z);        

        if (Physics.Raycast(start, transform.forward, out hit, distance, playerLayer))
        {
            return true;
        }
        Debug.DrawRay(start, transform.forward * distance, Color.red);
        return false;
    }

    public bool IsCoolDown(int i)
    {
        if (i == 1)
        {
            if (curCool1 > 0)
                return false;
            return true;
        }
        else if (i == 2)
        {
            if (curCool2 > 0)
                return false;
            return true;
        }
        return false;
    }

    public int CheckCoolDown()
    {
        if (curCool1 <= 0 && curCool2 <= 0)
            return 3;
        else if (curCool1 <= 0)
            return 1;
        else if (curCool2 <= 0)
            return 2;
        else
            return 0;
    }

    public float CalcDistance()
    {
        if (player != null)
        {
            Vector3 heading = player.transform.position - transform.position;
            float distance = heading.magnitude;
            return distance;
        }
        return 0;
    }

    public IEnumerator CoolDown1()
    {
        curCool1 = monsterData.CoolDown1;
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            curCool1 -= 0.1f;
            if (curCool1 <= 0)
                yield break;
        }
    }

    public IEnumerator CoolDown2()
    {
        curCool2 = monsterData.CoolDown2;
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            curCool2 -= 0.1f;
            if (curCool2 <= 0)
                yield break;
        }
    }

    private void Transition(IState state)
    {
        currentState = state;
        currentState.Enter();
    }

    public void WaitState()
    {
        Transition(waitState);
    }

    public void RunState()
    {
        Transition(runState);
    }

    public void AttackState1()
    {
        Transition(attackState1);
    }

    public void AttackState2()
    {
        Transition(attackState2);
    }

    public void StunState()
    {
        Transition(stunState);
    }

    public void DeathState()
    {
        Transition(deathState);
    }

    public void TestDamage(GameObject sender, float value)
    {
        throw new System.NotImplementedException();
    }

    public BlinkEffect GetBlinkEffect()
    {
        return blinkEffect;
    }
}
