using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPattern15_Imp : ActionNodeImplement
{
    private Vector3 center;

    protected override void Awake()
    {
        base.Awake();
        center = Vector3.zero;
    }
    protected override IEnumerator Act()
    {
        Debug.Log("Attack15 start");
        bossRPC.PlayAnimation("TakeOff");
        bossRPC.PlayJumpEffect();
        yield return new WaitForSeconds(bossRPC.clipDict["TakeOff"]);
        transform.position = center;
        bossRPC.PlayAnimation("Landing");
        yield return new WaitForSeconds(bossRPC.clipDict["Landing"] / 4);
        bossRPC.PlayJumpEffect();
        yield return new WaitForSeconds(bossRPC.clipDict["Landing"] / 4);

        yield return bossRPC.EnableShield();

        Debug.Log("Attack15 start");
        isEnd = true;
        action = null;
    }

}
