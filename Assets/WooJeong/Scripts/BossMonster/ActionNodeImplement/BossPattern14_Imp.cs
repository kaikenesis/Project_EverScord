using EverScord.Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPattern14_Imp : AttackNodeImplement
{
    public override NodeState Evaluate()
    {
        if (bossRPC.Phase == 1)
            return NodeState.FAILURE;
        return base.Evaluate();
    }

    protected override IEnumerator Act()
    {
        Debug.Log("Attack14 start");

        bossRPC.PlayAnimation("TakeOff");
        bossRPC.PlayJumpEffect();
        yield return new WaitForSeconds(bossRPC.clipDict["TakeOff"]);
        transform.position = bossRPC.SpawnPos;
        int randInt = Random.Range(0, 4);
        transform.rotation = Quaternion.identity;
        transform.Rotate(0, 90 * randInt, 0);
        Debug.Log($"{randInt} : {90 * randInt}");
        bossRPC.PlayAnimation("Landing");
        yield return new WaitForSeconds(bossRPC.clipDict["Landing"] * 0.25f);
        bossRPC.PlayJumpEffect();
        yield return new WaitForSeconds(bossRPC.clipDict["Landing"] * 0.75f);

        bossRPC.PlayAnimation("RotatingShot", 0);
        bossRPC.PlaySound("BossLaserReady");
        yield return new WaitForSeconds(1.5f);
        bossRPC.LaserEnable(bossRPC.clipDict["RotatingShot"] - 3f);
        bossRPC.PlaySound("BossPatternLaser");
        yield return new WaitForSeconds(bossRPC.clipDict["RotatingShot"] - 1.5f);
        bossRPC.StopSound("BossPatternLaser");
        bossRPC.PlayAnimation("Idle");
        isEnd = true;
        action = null;
        Debug.Log("Attack14 end");
    }
}
