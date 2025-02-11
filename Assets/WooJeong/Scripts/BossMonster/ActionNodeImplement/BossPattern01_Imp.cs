using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class BossPattern01_Imp : ActionNodeImplement
{

    protected override IEnumerator Action()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        Debug.Log("Attack1 start");
        //for(int i = 0; i < 7; i++)
        {
            
            //yield return new WaitForSeconds(0.15f);
        }
        yield return new WaitForSeconds(2f);
        isEnd = true;
        action = null;
        Debug.Log("Attack1 end");
    }
}
