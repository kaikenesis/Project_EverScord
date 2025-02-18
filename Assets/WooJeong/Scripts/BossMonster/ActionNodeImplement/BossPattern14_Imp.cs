using EverScord.Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPattern14_Imp : ActionNodeImplement
{
    protected override IEnumerator Act()
    {
        Debug.Log("Attack14 start");
        bossRPC.PlayAnimation("RotatingShot");
        yield return new WaitForSeconds(1.5f);
        bossRPC.LaserEnable(bossRPC.clipDict["RotatingShot"] - 3f);
        yield return new WaitForSeconds(bossRPC.clipDict["RotatingShot"] - 1.5f);
        bossRPC.PlayAnimation("Idle");
        isEnd = true;
        action = null;
        Debug.Log("Attack14 end");
    }
}
