using EverScord;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BossMove_Imp : ActionNodeImplement
{
    private NavMeshAgent navMeshAgent;

    private void Awake()
    {
        navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
    }

    public override void Setup(BossData bossData)
    {
        base.Setup(bossData);
        navMeshAgent.speed = bossData.Speed;
        navMeshAgent.stoppingDistance = bossData.StopDistance;
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
            action = StartCoroutine(Action());

        return NodeState.RUNNING;
    }

    protected override IEnumerator Action()
    {
        Debug.Log("move start");
        PlayAnimation("Walk");
        navMeshAgent.enabled = true;
        while (true)
        {
            navMeshAgent.destination = player.transform.position;
            if (CalcDistance() < navMeshAgent.stoppingDistance)
            {
                navMeshAgent.enabled = false;
                Debug.Log("move end");
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

        if(GameManager.Instance.playerPhotonViews.Count == 0)
        {
            player = null;
            return;
        }
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

    private float CalcDistance()
    {
        if (player == null)
            Debug.Log("player null");
        Vector3 heading = player.transform.position - transform.position;
        float distance = heading.magnitude;

        return distance;
    }
}
