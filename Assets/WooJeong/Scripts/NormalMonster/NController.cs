using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class NController : MonoBehaviour
{
    [SerializeField] protected float distance = 7.5f;
    [SerializeField] protected float moveSpeed = 3f;
    [SerializeField] protected float coolDown1 = 5;
    [SerializeField] protected float coolDown2 = 10;
    [SerializeField] protected float lookSpeed = 3f;

    [Tooltip("공격 사거리 표시 시간")]
    [SerializeField] protected float projectionTime = 1;

    [SerializeField] protected float attackRangeX = 0.5f;
    [SerializeField] protected float attackRangeY = 1f;
    [SerializeField] protected float attackRangeZ = 7.5f;
    protected float angle = 0.1f;

    [HideInInspector] public int LastAttack = 0;
    [HideInInspector] public GameObject player;
    private DecalProjector projector;
    private BoxCollider boxCollider;
    private Animator animator;
    public Dictionary<string, float> clipDict = new();
    private RaycastHit hit;

    private float curCool1 = 0;
    private float curCool2 = 0;

    public float MoveSpeed { get { return moveSpeed; } }
    public float Distance { get { return distance; } }
    public float ProjectionTime { get { return projectionTime; } }
    public float AttackRangeX { get { return attackRangeX; } }
    public float AttackRangeY { get { return attackRangeY; } }
    public float AttackRangeZ { get { return attackRangeZ; } }
    public DecalProjector Projector { get { return projector; } }
    public BoxCollider BoxCollider { get { return boxCollider; } }
    public Animator Animator { get { return animator; } }

    private IState currentState;
    private IState runState;
    private IState attackState1;
    private IState attackState2;
    private IState waitState;

    void Awake()
    {
        Setup();
        WaitState();
    }

    public virtual void Setup()
    {
        animator = GetComponentInChildren<Animator>();
        runState = gameObject.AddComponent<SK_112206_RunState>();
        attackState1 = gameObject.AddComponent<SK_112206_AttackState1>();
        attackState2 = gameObject.AddComponent<SK_112206_AttackState2>();
        waitState = gameObject.AddComponent<SK_112206_WaitState>();
        player = GameObject.Find("Player");
        projector = GetComponent<DecalProjector>();
        boxCollider = GetComponent<BoxCollider>();

        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            clipDict[clip.name] = clip.length;
        }
    }

    public void LookPlayer()
    {
        Vector3 dir = player.transform.position - transform.position;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * lookSpeed);
        transform.rotation = new(0, transform.rotation.y, 0, transform.rotation.w);
    }

    public bool IsLookPlayer()
    {
        //Vector3 start = new(transform.position.x, transform.position.y + 0.3f, transform.position.z);
        //if(Physics.Raycast(start, Vector3.forward, out hit, distance))
        //{
        //    Debug.Log(hit.transform.name);
        //    if (hit.transform.CompareTag("Player"))
        //    {
        //        return true;
        //    }
        //}
        //Debug.DrawRay(start, transform.forward * distance, Color.red);

        Vector3 dir = player.transform.position - transform.position;
        Quaternion lookRot = Quaternion.LookRotation(dir);
        Quaternion minRot = new(0, lookRot.y - angle, 0, lookRot.w);
        Quaternion maxRot = new(0, lookRot.y + angle, 0, lookRot.w);
        if (transform.rotation.y > minRot.y &&
            transform.rotation.y < maxRot.y)
        {
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
        Vector3 heading = player.transform.position - transform.position;
        float distance = heading.magnitude;

        return distance;
    }

    public IEnumerator CoolDown1()
    {
        curCool1 = coolDown1;
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
        curCool2 = coolDown2;
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            curCool2 -= 0.1f;
            if (curCool2 <= 0)
                yield break;
        }
    }

    public void Transition(IState state)
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
}
