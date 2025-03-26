using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPattern15_Imp : AttackNodeImplement
{
    private bool isUsed = false;

    protected override void Awake()
    {
        base.Awake();
    }

    public override NodeState Evaluate()
    {
        if (isUsed)
            return NodeState.FAILURE;
        if (bossRPC.Phase == 1)
            return NodeState.FAILURE;
        if (!bossRPC.IsUnderHP(20))
            return NodeState.FAILURE;

        if (isEnd)
        {
            isEnd = false;
            action = null;
            isUsed = true;
            return NodeState.SUCCESS;
        }

        if (action == null)
            action = StartCoroutine(nameof(Act));

        return NodeState.RUNNING;
    }

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
