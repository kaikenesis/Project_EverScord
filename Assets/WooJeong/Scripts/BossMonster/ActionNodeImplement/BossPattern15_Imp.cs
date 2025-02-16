using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPattern15_Imp : ActionNodeImplement
{
    private Vector3 center;

    protected override void Awake()
    {
        base.Awake();
        center = transform.position;
    }
    protected override IEnumerator Act()
    {
        bossRPC.PlayAnimation("TakeOff");
        yield return new WaitForSeconds(bossRPC.clipDict["TakeOff"]);
        transform.position = center;
        bossRPC.PlayAnimation("Landing");
        yield return new WaitForSeconds(bossRPC.clipDict["Landing"]);
        yield return bossRPC.EnableShield();
        isEnd = true;
        action = null;
        yield return null;
    }

}
