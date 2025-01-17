using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMove_Imp : MonoBehaviour
{
    private GameObject player;
    private float speed;
    private float distance;
    private bool isClose = false;
    private Coroutine move;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void Init(float sp, float dis)
    {
        speed = sp; 
        distance = dis;
    }

    public NodeState Evaluate()
    {
        if(isClose)
            return NodeState.SUCCESS;

        if(move != null)
            return NodeState.RUNNING;
        else
            move = StartCoroutine(Move());
        
        return NodeState.RUNNING;
    }

    private IEnumerator Move()
    {
        isClose = false;
        while (true)
        {
            Vector3 vec = (player.transform.position - transform.position);
            if (vec.magnitude < distance)
            {
                isClose = true;
                move = null;
                yield break;
            }
            Vector3 moveVector = vec.normalized;
            transform.Translate(speed * Time.deltaTime * moveVector, Space.World);
            yield return new WaitForSeconds(Time.deltaTime);
        }        
    }
}
