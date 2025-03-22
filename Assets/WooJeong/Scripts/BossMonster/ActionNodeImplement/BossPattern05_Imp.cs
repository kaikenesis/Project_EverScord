using EverScord.Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPattern05_Imp : AttackNodeImplement
{
    private float laserLifeTime = 3.5f;
    protected int failurePhase = 2;

    protected override void Awake()
    {
        base.Awake();
        attackableHP = 70;
    }

    public override NodeState Evaluate()
    {
        if (bossRPC.Phase == failurePhase)
            return NodeState.FAILURE;
        return base.Evaluate();
    }

    protected override IEnumerator Act()
    {
        Debug.Log("Attack5 start");

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
        yield return new WaitForSeconds(1.5f);
        bossRPC.LaserEnable(laserLifeTime);
        bossRPC.PlaySound("BossLaserReady");
        bossRPC.PlaySound("BossPatternLaser");
        yield return new WaitForSeconds(laserLifeTime);
        bossRPC.StopSound("BossPatternLaser");
        bossRPC.PlayAnimation("Idle");
        isEnd = true;
        action = null;
        Debug.Log("Attack5 end");
    }
}
