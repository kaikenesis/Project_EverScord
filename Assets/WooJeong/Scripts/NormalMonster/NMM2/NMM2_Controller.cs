using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class NMM2_Controller : NController
{
    public override BodyType EnemyBodyType => BodyType.METAL;
    protected override void Setup()
    {
        monsterType = MonsterType.MEDIUM;
        runState = gameObject.AddComponent<NMM2_RunState>();
        attackState1 = gameObject.AddComponent<NMM2_AttackState1>();
        attackState2 = gameObject.AddComponent<NMM2_AttackState2>();
        waitState = gameObject.AddComponent<NMM2_WaitState>();
        stunState = gameObject.AddComponent<NMM2_StunState>();
        deathState = gameObject.AddComponent<NMM2_DeathState>();
    }
    protected override void SetHealthBar()
    {
        base.SetHealthBar();
        monsterHealthBar.SetOffsetY(4);
    }
}
