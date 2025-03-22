using EverScord;
using EverScord.Character;
using EverScord.Effects;
using EverScord.Pool;
using EverScord.Skill;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum MonsterType
{
    SMALL, MEDIUM, LARGE
}

public abstract class NController : MonoBehaviour, IEnemy
{
    public NMonsterData monsterData;

    public float HP {  get; private set; }
    private const float SMALL_SIZE = 2f;
    private const float MEDIUM_SIZE = 3f;
    private const float LARGE_SIZE = 4f;

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

    public BoxCollider Hitbox { get; protected set; }
    public BoxCollider BoxCollider1 { get; protected set; }
    public BoxCollider BoxCollider2 { get; protected set; }
    public Animator Animator { get; protected set; }
    public NavMeshAgent MonsterNavMeshAgent { get; protected set; }

    public string GUID { get; protected set; }

    private BlinkEffect blinkEffect;
    private DissolveEffect dissolveEffect;
    private UIMarker uiMarker;

    public PhotonView PhotonView => photonView;

    public virtual BodyType EnemyBodyType => BodyType.FLESH;

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
        Hitbox = GetComponent<BoxCollider>();
        MonsterNavMeshAgent = GetComponent<NavMeshAgent>();
        BoxCollider1 = gameObject.AddComponent<BoxCollider>();
        BoxCollider2 = gameObject.AddComponent<BoxCollider>();
        uiMarker = gameObject.AddComponent<UIMarker>();
        uiMarker.Initialize(PointMarkData.EType.Monster);
        ColliderSetup();

        blinkEffect = BlinkEffect.Create(this);
        dissolveEffect = DissolveEffect.Create(this);


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

    protected void ColliderSetup()
    {
        BoxCollider1.center = new Vector3(0, transform.position.y,
                                          monsterData.Skill01_RangeZ / 2);

        BoxCollider1.size = new Vector3(monsterData.Skill01_RangeX,
                                        monsterData.Skill01_RangeY,
                                        monsterData.Skill01_RangeZ);

        BoxCollider2.center = new Vector3(0, transform.position.y,
                                        monsterData.Skill02_RangeZ / 2);

        BoxCollider2.size = new Vector3(monsterData.Skill02_RangeX,
                                        monsterData.Skill02_RangeY,
                                        monsterData.Skill02_RangeZ);

        BoxCollider1.isTrigger = true;
        BoxCollider2.isTrigger = true;
        BoxCollider1.enabled = false;
        BoxCollider2.enabled = false;
    }

    private void Start()
    {
        LevelControl.OnProgressUpdated += ProgressCheck;
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        LevelControl.OnProgressUpdated -= ProgressCheck;
    }

    private void OnEnable()
    {
        isDead = false;
        LastAttack = 0;
        uiMarker.SetActivate(true);
        HP = monsterData.HP;
        if (healthBarObject == null)
        {
            SetHealthBar();
        }
        healthBarObject.SetActive(true);
        monsterHealthBar.InitHealthBar(monsterData.HP);        
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
        if (currentProgress >= 1.0f)
        {
            // 현재 진행도 체크하고 다 됐으면 죽임
            isDead = true;
        }
    }

    public void PlaySound(string soundName, float volume = 1.0f)
    {
        photonView.RPC(nameof(SyncNMSound), RpcTarget.All, soundName, volume);
    }

    [PunRPC]
    protected void SyncNMSound(string soundName, float volume)
    {
        SoundManager.Instance.PlaySound(soundName, volume);
    }

    public void StopSound(string soundName)
    {
        photonView.RPC(nameof(SyncStopSound), RpcTarget.All, soundName);
    }

    [PunRPC]
    protected void SyncStopSound(string soundName)
    {
        SoundManager.Instance.StopSound(soundName);
    }

    protected virtual void SetHealthBar()
    {
        if (healthBarObject != null)
            return;

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
            float totalDamage;
            CharacterControl controller = other.GetComponent<CharacterControl>();
            if (LastAttack == 1)
                totalDamage = DamageCalculator.GetSkillDamage(monsterData.BaseAttackDamage, monsterData.Skill01_Damage, 0, 0, controller.Stats.Defense);
            else
                totalDamage = DamageCalculator.GetSkillDamage(monsterData.BaseAttackDamage, monsterData.Skill02_Damage, 0, 0, controller.Stats.Defense);
            controller.DecreaseHP(totalDamage);
        }
    }

    public void SetGUID(string guid)
    {
        GUID = guid;
    }

    public void InstantiateMonsterAttack(Vector3 pos, float width, float projectTime, string addressableKey, float skillDamage, string soundName)
    {
        photonView.RPC(nameof(SyncMonsterAttack), RpcTarget.All, pos, width, projectTime, addressableKey, skillDamage, soundName);
    }

    [PunRPC]
    protected void SyncMonsterAttack(Vector3 pos, float width, float projectTime, string addressableKey, float skillDamage, string soundName)
    {
        GameObject go = ResourceManager.Instance.GetFromPool("MonsterAttack", pos, Quaternion.identity);
        MonsterAttack ma = go.GetComponent<MonsterAttack>();
        ma.Setup(width, projectTime, addressableKey, monsterData.BaseAttackDamage, skillDamage, soundName);
    }

    public void DecreaseHP(float damage, CharacterControl attacker)
    {
        this.HP -= damage;
        if (this.HP <= 0)
        {
            isDead = true;
            attacker.IncreaseKillCount();
            GameManager.Instance.LevelController.IncreaseMonsterProgress(monsterType);
        }

        if (monsterHealthBar != null)
            monsterHealthBar.UpdateHealth(damage);

        photonView.RPC(nameof(SyncMonsterHP), RpcTarget.Others, damage);
    }

    [PunRPC]
    protected void SyncMonsterHP(float damage)
    {
        this.HP -= damage;
        if (this.HP <= 0)
        {
            isDead = true;
            GameManager.Instance.LevelController.IncreaseMonsterProgress(monsterType);
        }

        if (monsterHealthBar != null)
            monsterHealthBar.UpdateHealth(damage);
    }

    public void StunMonster(float stunTime)
    {
        photonView.RPC(nameof(SyncMonsterStun), RpcTarget.All, stunTime);
    }
    

    [PunRPC]
    protected void SyncMonsterStun(float stunTime)
    {
        isStun = true;
        this.stunTime = stunTime;
    }

    [PunRPC]
    public void SyncDissolve(float duration)
    {
        StartCoroutine(dissolveEffect.Dissolve(duration));
    }

    [PunRPC]
    public void DeathAftermath()
    {
        DeathGlitter();

        if (healthBarObject)
            healthBarObject.SetActive(false);
    }

    private void DeathGlitter()
    {
        PooledParticle glitter = ResourceManager.Instance.GetFromPool(AssetReferenceManager.DeathGlitter_ID) as PooledParticle;
        glitter.Init(AssetReferenceManager.DeathGlitter_ID);
        glitter.transform.position = transform.position;

        float effectSize;

        switch (monsterType)
        {
            case MonsterType.SMALL:
                effectSize = SMALL_SIZE;
                break;

            case MonsterType.MEDIUM:
                effectSize = MEDIUM_SIZE;
                break;

            case MonsterType.LARGE:
                effectSize = LARGE_SIZE;
                break;

            default:
                effectSize = MEDIUM_SIZE;
                break;
        }
        glitter.transform.localScale = new Vector3(effectSize, effectSize, effectSize);
        glitter.Play();
    }

    public void Death()
    {
        photonView.RPC(nameof(SyncMonsterDeath), RpcTarget.All);        
    }

    [PunRPC]
    protected void SyncMonsterDeath()
    {
        isDead = false;
        ResourceManager.Instance.ReturnToPool(gameObject, GUID);
    }

    public virtual void StartFSM()
    {
        SetActiveHitbox(true);
        WaitState();
    }

    public void PlayAnimation(string animationName)
    {
        Animator.CrossFade(animationName, 0.3f, -1, 0);
        photonView.RPC(nameof(SyncAnimation), RpcTarget.Others, animationName);
    }

    [PunRPC]
    protected void SyncAnimation(string animationName)
    {
        Animator.CrossFade(animationName, 0.3f, -1, 0);
    }

    public void Fire(string projectileName, float skillDamage)
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

        mp.Setup(projectileName, id, position, transform.forward, monsterData.BaseAttackDamage, skillDamage, projectileSpeed);
        photonView.RPC(nameof(SyncProjectileNM), RpcTarget.Others, projectileName, id, position, transform.forward, skillDamage, projectileSpeed);
    }

    [PunRPC]
    protected void SyncProjectileNM(string projectileName, int id, Vector3 position, Vector3 direction, float damage, float projectileSpeed)
    {
        GameObject go = ResourceManager.Instance.GetFromPool("MonsterProjectile", position, Quaternion.identity);
        MonsterProjectile mp = go.GetComponent<MonsterProjectile>();
        GameManager.Instance.ProjectileController.AddDict(id, mp);
        mp.Setup(projectileName, id, position, direction, monsterData.BaseAttackDamage, damage, projectileSpeed);
    }

    public void SetActiveHitbox(bool value)
    {
        photonView.RPC(nameof(SyncSetActiveHitBox), RpcTarget.All, value);
    }

    [PunRPC]
    protected void SyncSetActiveHitBox(bool value)
    {
        Hitbox.enabled = value;
    }

    public IEnumerator GameEnd()
    {
        yield return new WaitForSeconds(10f);
        Death();
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

        player = nearPlayer;
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

    public float GetDefense()
    {
        return monsterData.Defense;
    }

    public void SetDebuff(CharacterControl attacker, EBossDebuff debuffState, float time, float value)
    {
        GameManager.Instance.DebuffSystem.SetDebuff(this, debuffState, attacker, time, value);
    }

    public void SetDebuff()
    {
        throw new NotImplementedException();
    }

    public NavMeshAgent GetNavMeshAgent()
    {
        return MonsterNavMeshAgent;
    }
}
