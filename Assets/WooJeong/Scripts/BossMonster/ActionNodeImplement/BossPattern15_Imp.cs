using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPattern15_Imp : ActionNodeImplement
{
    protected override IEnumerator Act()
    {
        StartCoroutine(nameof(CheckDeath));
        bossRPC.PlayAnimation("TakeOff");
        bossRPC.PlayJumpEffect();
        yield return new WaitForSeconds(bossRPC.clipDict["TakeOff"]);
        transform.position = bossRPC.SpawnPos;
        bossRPC.PlayAnimation("Landing");
        yield return new WaitForSeconds(bossRPC.clipDict["Landing"] / 4);
        bossRPC.PlayJumpEffect();
        yield return new WaitForSeconds(bossRPC.clipDict["Landing"] / 4);

        bossRPC.PlaySound("BossPattern15");
        yield return bossRPC.EnableShield();
        bossRPC.StopSound("BossPattern15");

        isEnd = true;
        action = null;
    }
}
