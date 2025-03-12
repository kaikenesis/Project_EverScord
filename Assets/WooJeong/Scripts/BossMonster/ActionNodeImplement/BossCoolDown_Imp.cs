using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCoolDown_Imp : ActionNodeImplement
{
    private bool isUnder90 = false;
    private float coolDown1 = 5;
    private float coolDown2 = 10;

    protected override IEnumerator Act()
    {
        Debug.Log("CoolDown start");
        if(isUnder90 == false && bossRPC.IsUnderHP(90) == true)
        {
            isUnder90 = true;
        }
        if(!isUnder90)
        {
            yield return new WaitForSeconds(coolDown1);
            isEnd = true;
            action = null;
            yield break;
        }
        yield return new WaitForSeconds(coolDown2);
        isEnd = true;
        action = null;
        Debug.Log("CoolDown end");
    }
}
