using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SK_112206_Controller : MonoBehaviour
{
    [SerializeField] private float distance = 7.5f;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float coolDown1 = 5;
    [SerializeField] private float coolDown2 = 10;    
    [SerializeField] private float lookSpeed = 3f;
    [Tooltip("공격 사거리 표시 시간")]
    [SerializeField] private float projectionTime = 1;
    [SerializeField] private float attackRangeX = 0.5f;
    [SerializeField] private float attackRangeY = 1f;
    [SerializeField] private float attackRangeZ = 7.5f;

    public float MoveSpeed { get { return moveSpeed; } }
    public float Distance { get { return distance; } }
    public float CoolDown1 { get { return coolDown1; } }
    public float CoolDown2 { get { return coolDown2; } }
    public float ProjectionTime { get { return projectionTime; } }
    public float AttackRangeX { get { return attackRangeX; } }
    public float AttackRangeY { get { return attackRangeY; } }
    public float AttackRangeZ { get { return attackRangeZ; } }

    [HideInInspector] public GameObject player;
    private DecalProjector projector;
    public DecalProjector Projector { get { return projector; } }

    private Animator animator;
    public Animator Animator { get { return animator; } }
    public Dictionary<string, float> clipDict = new();

    private IState currentState;

    private IState moveState;
    private IState attackState1;
    private IState attackState2;
    private IState idleState;

    private ICoolDown cool1;
    private ICoolDown cool2;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        moveState = gameObject.AddComponent<SK_112206_RunState>();
        attackState1 = gameObject.AddComponent<SK_112206_AttackState1>();
        attackState2 = gameObject.AddComponent<SK_112206_AttackState2>();       
        idleState = gameObject.AddComponent<SK_112206_WaitState>();
        player = GameObject.Find("Player");
        projector = GetComponent<DecalProjector>();

        cool1 = GetComponent<SK_112206_AttackState1>();
        cool2 = GetComponent<SK_112206_AttackState2>();

        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            clipDict[clip.name] = clip.length;
        }

        IdleState();
    }

    public void LookPlayer()
    {
        Vector3 dir = player.transform.position - this.transform.position;

        this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * lookSpeed);
    }

    public int CheckCoolDown()
    {
        if (cool1.CurCool <= 0 && cool2.CurCool <= 0)
            return 3;
        else if (cool1.CurCool <= 0)
            return 1;
        else if (cool2.CurCool <= 0)
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

    public void Transition(IState state)
    {
        currentState = state;
        currentState.Enter();
    }

    public void IdleState()
    {
        Transition(idleState);
    }

    public void MoveState()
    {
        Transition(moveState);
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
