using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SK_112301_Controller : NController
{
    private float chargeRange;

    public float ChargeRange { get { return chargeRange; } }

    protected override void Setup()
    {
        runState = gameObject.AddComponent<SK_112301_RunState>();
        attackState1 = gameObject.AddComponent<SK_112301_AttackState1>();
        attackState2 = gameObject.AddComponent<SK_112301_AttackState2>();
        waitState = gameObject.AddComponent<SK_112301_WaitState>();
        stunState = gameObject.AddComponent<SK_112301_StunState>();
        deathState = gameObject.AddComponent<SK_112301_DeathState>();

        var temp = monsterData as NMD_112301;
        chargeRange = temp.ChargeRange;

        Projector2.size = new Vector3(monsterData.AttackRangeX2,
                              monsterData.AttackRangeY2,
                              chargeRange);

        Projector2.pivot = new Vector3(0, transform.position.y,
                                       chargeRange / 2);
    }
}
