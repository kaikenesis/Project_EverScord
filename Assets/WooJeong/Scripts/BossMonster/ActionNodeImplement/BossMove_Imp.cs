using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class BossMove_Imp : ActionNodeImplement
{
    private GameObject player;
    private float speed;
    private float distance;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public override void Setup(BossData bossData)
    {
        speed = bossData.Speed;
        distance = bossData.AttackRange;
    }

    protected override IEnumerator Action()
    {
        Debug.Log("move start");
        while (true)
        {
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
        }        
    }
}
