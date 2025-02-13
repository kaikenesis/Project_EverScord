using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPattern06_Imp : ActionNodeImplement
{
    protected override IEnumerator Act()
    {
        Debug.Log("Attack6 start");
        bossRPC.PlayAnimation("RotatingShot");
        yield return new WaitForSeconds(1f);
        yield return bossRPC.LaserEnable(2f);
        yield return new WaitForSeconds(bossRPC.clipDict["RotatingShot"] - 3f);
        isEnd = true;
        action = null;
        Debug.Log("Attack6 end");
    }
}
