using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class NML1_Controller : NController
{
    private float chargeRange;

    public float ChargeRange { get { return chargeRange; } }

    protected override void Setup()
    {
        monsterType = MonsterType.LARGE;
        runState = gameObject.AddComponent<NML1_RunState>();
        attackState1 = gameObject.AddComponent<NML1_AttackState1>();
        attackState2 = gameObject.AddComponent<NML1_AttackState2>();
        waitState = gameObject.AddComponent<NML1_WaitState>();
        stunState = gameObject.AddComponent<NML1_StunState>();
        deathState = gameObject.AddComponent<NML1_DeathState>();

        var temp = monsterData as NML1_Data;
        chargeRange = temp.ChargeRange;

        Projector2.size = new Vector3(monsterData.AttackRangeX2,
                              monsterData.AttackRangeY2,
                              chargeRange);

        Projector2.pivot = new Vector3(0, transform.position.y,
                                       chargeRange / 2);
    }
}
