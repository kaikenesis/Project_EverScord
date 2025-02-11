using EverScord;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BossMove_Imp : ActionNodeImplement
{
    private GameObject player;
    private NavMeshAgent navMeshAgent;

    private void Awake()
    {
        navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
    }

    public override void Setup(BossData bossData, Animator animator)
    {
        base.Setup(bossData, animator);
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
        navMeshAgent.enabled = true;
        while (true)
        {
            /*
            Vector3 vec = (player.transform.position - transform.position);
            if (vec.magnitude < distance)
            {
                Debug.Log("move end");
                isEnd = true;
                action = null;
                yield break;
            }
            Vector3 moveVector = vec.normalized;
            transform.Translate(speed * Time.deltaTime * moveVector, Space.World);
            yield return new WaitForSeconds(Time.deltaTime);
            */
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
