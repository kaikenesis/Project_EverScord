using EverScord.Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPattern06_Imp : ActionNodeImplement
{
    private float laserLifeTime = 5;

    protected override IEnumerator Act()
    {
        Debug.Log("Attack6 start");
        bossRPC.PlayAnimation("RotatingShot");
        yield return new WaitForSeconds(1f);
        yield return bossRPC.LaserEnable(laserLifeTime);
        yield return new WaitForSeconds(bossRPC.clipDict["RotatingShot"] - laserLifeTime - 1);
        isEnd = true;
        action = null;
        Debug.Log("Attack6 end");
    }

}
