using EverScord.Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPattern06_Imp : ActionNodeImplement
{
    private float laserLifeTime = 3.5f;

    protected override void Awake()
    {
        base.Awake();
        attackableHP = 80;
    }

    protected override IEnumerator Act()
    {
        Debug.Log("Attack6 start");
        bossRPC.PlayAnimation("RotatingShot", 0);
        yield return new WaitForSeconds(1.5f);
        bossRPC.LaserEnable(laserLifeTime);
        yield return new WaitForSeconds(laserLifeTime);
        bossRPC.PlayAnimation("Idle");
        isEnd = true;
        action = null;
        Debug.Log("Attack6 end");
    }
}
