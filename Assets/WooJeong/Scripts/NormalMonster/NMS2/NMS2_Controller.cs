using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NMS2_Controller : NController
{
    public bool isUpgraded = false;

    protected override void Setup()
    {
        monsterType = MonsterType.SMALL;
        runState =      gameObject.AddComponent<NMS2_RunState>();
        attackState1 =  gameObject.AddComponent<NMS2_AttackState1>();
        attackState2 =  gameObject.AddComponent<NMS2_AttackState2>();
        waitState =     gameObject.AddComponent<NMS2_WaitState>();
        stunState =     gameObject.AddComponent<NMS2_StunState>();
        deathState =    gameObject.AddComponent<NMS2_DeathState>();
    }

    protected override void SetHealthBar()
    {
        base.SetHealthBar();
        monsterHealthBar.SetOffsetY(3);
    }

    public override void StartFSM()
    {
        base.StartFSM();
        PlaySound("NMS2_Spawn");
    }
}
