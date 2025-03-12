using DTT.AreaOfEffectRegions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class NML1_Controller : NController
{
    public GameObject LineIndicator;
    public SRPLineRegionProjector LineProjector;

    protected override void Setup()
    {
        monsterType = MonsterType.LARGE;
        runState = gameObject.AddComponent<NML1_RunState>();
        attackState1 = gameObject.AddComponent<NML1_AttackState1>();
        attackState2 = gameObject.AddComponent<NML1_AttackState2>();
        waitState = gameObject.AddComponent<NML1_WaitState>();
        stunState = gameObject.AddComponent<NML1_StunState>();
        deathState = gameObject.AddComponent<NML1_DeathState>();
    }

    protected override void SetHealthBar()
    {
        base.SetHealthBar();
        monsterHealthBar.SetOffsetY(5);
    }
}
