using EverScord;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BossMove_Imp : ActionNodeImplement
{
    private GameObject player;
    private float speed;
    private float distance;
    private NavMeshAgent navMeshAgent;

    private void Awake()
    {
        navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
    }

    public override void Setup(BossData bossData)
    {
        speed = bossData.Speed;
        distance = bossData.AttackRange;
    }

    protected override IEnumerator Action()
    {
        Debug.Log("move start");
        navMeshAgent.enabled = true;
        navMeshAgent.destination = player.transform.position;
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
            if(navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance)
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
}
