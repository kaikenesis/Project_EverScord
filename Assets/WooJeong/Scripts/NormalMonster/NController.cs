using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public abstract class NController : MonoBehaviour
{
    [SerializeField] private Material decalMat;
    [SerializeField] protected float distance = 7.5f;
    [SerializeField] protected float moveSpeed = 3f;
    [SerializeField] protected float coolDown1 = 5;
    [SerializeField] protected float coolDown2 = 10;
    [SerializeField] protected float lookSpeed = 3f;

    [Header("공격 사거리 표시 시간")]
    [SerializeField] protected float projectionTime = 1;
    [Header("공격1 사거리")]
    [SerializeField] protected float attackRangeX1 = 0.5f;
    [SerializeField] protected float attackRangeY1 = 1f;
    [SerializeField] protected float attackRangeZ1 = 7.5f;

    protected float angle = 0.1f;

    [HideInInspector] public int LastAttack = 0;
    [HideInInspector] public GameObject player;
    protected DecalProjector projector;
    protected BoxCollider boxCollider;
    protected Animator animator;
    public Dictionary<string, float> clipDict = new();
    protected RaycastHit hit;

    protected float curCool1 = 0;
    protected float curCool2 = 0;

    public float MoveSpeed { get { return moveSpeed; } }
    public float Distance { get { return distance; } }
    public float ProjectionTime { get { return projectionTime; } }
    public float AttackRangeX1 { get { return attackRangeX1; } }
    public float AttackRangeY1 { get { return attackRangeY1; } }
    public float AttackRangeZ1 { get { return attackRangeZ1; } }

    public DecalProjector Projector { get { return projector; } }
    public BoxCollider BoxCollider { get { return boxCollider; } }
    public Animator Animator { get { return animator; } }

    protected IState currentState;
    protected IState runState;
    protected IState attackState1;
    protected IState attackState2;
    protected IState waitState;

    void Awake()
    {
        Setup();
        WaitState();
    }

    protected abstract void Setup();

    protected void ProjectorSetup()
    {
        projector.renderingLayerMask = 2;
        projector.material = decalMat;
    }

    public void LookPlayer()
    {
        Vector3 dir = player.transform.position - transform.position;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * lookSpeed);
        transform.rotation = new(0, transform.rotation.y, 0, transform.rotation.w);
    }

    public bool IsLookPlayer()
    {
        Vector3 start = new(transform.position.x, transform.position.y + 0.3f, transform.position.z);        

        if (Physics.Raycast(start, transform.forward, out hit, distance))
        {
            if (hit.transform.CompareTag("Player"))
            {
                return true;
            }
        }

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
}
