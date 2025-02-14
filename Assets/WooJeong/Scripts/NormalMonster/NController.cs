using EverScord;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public abstract class NController : MonoBehaviour, IEnemy
{
    [SerializeField] public NMonsterData monsterData;

    [HideInInspector] public int LastAttack = 0;
    [HideInInspector] public GameObject player;
    public Dictionary<string, float> clipDict = new();
    public float HP = 0;
    public float stunTime = 2;
    public bool isStun = false;
    public bool isDead = false;
    protected float curCool1 = 0;
    protected float curCool2 = 0;
    protected RaycastHit hit;
    [SerializeField] protected LayerMask playerLayer;

    public DecalProjector Projector1 { get; protected set; }
    public DecalProjector Projector2 { get; protected set; }
    public BoxCollider BoxCollider1 { get; protected set; }
    public BoxCollider BoxCollider2 { get; protected set; }
    public Animator Animator { get; protected set; }
    public string GUID { get; protected set; }

    private PhotonView photonView;

    protected IState currentState;
    protected IState runState;
    protected IState attackState1;
    protected IState attackState2;
    protected IState waitState;
    protected IState stunState;
    protected IState deathState;

    protected void Awake()
    {
        _ = ResourceManager.Instance.CreatePool("MonsterAttack");
        _ = ResourceManager.Instance.CreatePool("NML2_A1_Effect01");
        _ = ResourceManager.Instance.CreatePool("NML2_A2_Effect");

        HP = monsterData.HP;
        photonView = GetComponent<PhotonView>();
        Animator = GetComponentInChildren<Animator>();

        Projector1 = gameObject.AddComponent<DecalProjector>();
        Projector2 = gameObject.AddComponent<DecalProjector>();
        BoxCollider1 = gameObject.AddComponent<BoxCollider>();
        BoxCollider2 = gameObject.AddComponent<BoxCollider>();
        ProjectorSetup();

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

        if (!PhotonNetwork.IsMasterClient)
            return;

        playerLayer = LayerMask.GetMask("Player");
        SetNearestPlayer();
        
        Setup();
    }

    public void SetGUID(string guid)
    {
        GUID = guid;
    }

    public void InstantiateMonsterAttack(Vector3 pos, float width, float projectTime, string addressableKey)
    {
        photonView.RPC("SyncMonsterAttack", RpcTarget.All, pos, width, projectTime, addressableKey);
    }

    [PunRPC]
    protected void SyncMonsterAttack(Vector3 pos, float width, float projectTime, string addressableKey)
    {
        GameObject go = ResourceManager.Instance.GetFromPool("MonsterAttack", pos, Quaternion.identity);
        MonsterAttack ma = go.GetComponent<MonsterAttack>();
        ma.Setup(width, projectTime, addressableKey);
    }

    public void DecreaseHP(float hp)
    {
        photonView.RPC("SyncMonsterHP", RpcTarget.All, hp);
    }

    [PunRPC]
    protected void SyncMonsterHP(float hp)
    {
        HP -= hp;
        if (HP <= 0)
            isDead = true;
    }

    public void StartFSM()
    {
        if (PhotonNetwork.IsMasterClient)
            WaitState();
    }

    protected abstract void Setup();

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

    public void PlayAnimation(string animationName)
    {
        Animator.CrossFade(animationName, 0.3f, -1, 0); // 로컬에서 애니메이션 실행
        photonView.RPC("SyncAnimation", RpcTarget.Others, animationName); // 다른 클라이언트에도 애니메이션 실행 요청
    }

    [PunRPC]
    protected void SyncAnimation(string animationName)
    {
        Animator.CrossFade(animationName, 0.3f, -1, 0); // 받은 클라이언트에서 동일한 애니메이션 실행
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

    public void SetNearestPlayer()
    {
        float nearest = Mathf.Infinity;
        GameObject nearPlayer = null;

        foreach (var player in GameManager.Instance.playerPhotonViews)
        {
            float cur = (player.transform.position - transform.position).magnitude;
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
        Vector3 dir = player.transform.position - transform.position;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * monsterData.SmoothAngleSpeed);
        transform.rotation = new(0, transform.rotation.y, 0, transform.rotation.w);
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
        if (player == null)
            Debug.Log("player null");
        Vector3 heading = player.transform.position - transform.position;
        float distance = heading.magnitude;

        return distance;
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

}
