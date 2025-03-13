using EverScord;
using EverScord.Character;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BossMove_Imp : ActionNodeImplement
{
    private NavMeshAgent navMeshAgent;
    protected LayerMask playerLayer;
    protected GameObject player;

    protected override void Awake()
    {
        base.Awake();
        navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
        navMeshAgent.speed = bossRPC.BossMonsterData.Speed1;
        navMeshAgent.stoppingDistance = bossRPC.BossMonsterData.StopDistance;
        playerLayer = LayerMask.GetMask("Player");
    }

    public override NodeState Evaluate()
    {
        if (isEnd)
        {
            isEnd = false;
            return NodeState.SUCCESS;
        }
        
        SetNearestPlayer();
        if(player == null)
        {
            return NodeState.FAILURE;
        }

        if (action == null)
            action = StartCoroutine(Act());

        return NodeState.RUNNING;
    }

    protected override IEnumerator Act()
    {
        Debug.Log("Move Start");
        bossRPC.PlayAnimation("Walk");
        navMeshAgent.enabled = true;
        while (true)
        {
            if (player != null)
            {
                navMeshAgent.destination = player.transform.position;
            }
            else
                SetNearestPlayer();

            float distance = CalcDistance();
            LookPlayer();
            if (distance < navMeshAgent.stoppingDistance && IsLookPlayer(navMeshAgent.stoppingDistance + 1))
            {
                Debug.Log("Move end");
                navMeshAgent.enabled = false;
                isEnd = true;
                action = null;
                yield break;
            }
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    private void SetNearestPlayer()
    {
        float nearest = Mathf.Infinity;
        GameObject nearPlayer = null;

        foreach (var kv in GameManager.Instance.PlayerDict)
        {
            CharacterControl player = kv.Value;

            if (player.IsDead)
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

    private float CalcDistance()
    {
        if (player != null)
        {
            Vector3 heading = player.transform.position - transform.position;
            float distance = heading.magnitude;
            return distance;
        }

        return 0;
    }

    public void LookPlayer()
    {
        if (player != null)
        {
            Vector3 dir = player.transform.position - transform.position;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 20);
            transform.rotation = new(0, transform.rotation.y, 0, transform.rotation.w);
        }
    }

    public bool IsLookPlayer(float distance)
    {
        Vector3 start = new(transform.position.x, transform.position.y + 0.3f, transform.position.z);
        if (Physics.Raycast(start, transform.forward, distance, playerLayer))
        {
            return true;
        }
        Debug.DrawRay(start, transform.forward * distance, Color.red);
        return false;
    }
}
