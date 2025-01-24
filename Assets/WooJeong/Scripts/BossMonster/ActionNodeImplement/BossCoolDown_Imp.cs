using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossCoolDown_Imp : ActionNodeImplement
{
    float coolDown = 10;
    float curCool = 0;
    protected override IEnumerator Action()
    {
        Debug.Log("CoolDown start");
        yield return new WaitForSeconds(coolDown);
        isEnd = true;
        action = null;
        Debug.Log("CoolDown end");
    }
}
