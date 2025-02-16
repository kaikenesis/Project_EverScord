using EverScord.Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPattern14_Imp : ActionNodeImplement
{
    private float laserLifeTime = 3.5f;

    protected override IEnumerator Act()
    {
        Debug.Log("Attack6 start");
        bossRPC.PlayAnimation("RotatingShot");
        yield return new WaitForSeconds(1.5f);
        bossRPC.LaserEnable(bossRPC.clipDict["RotatingShot"]);
        yield return new WaitForSeconds(bossRPC.clipDict["RotatingShot"] - 1.5f);
        bossRPC.PlayAnimation("Idle");
        isEnd = true;
        action = null;
        Debug.Log("Attack6 end");
    }
}
