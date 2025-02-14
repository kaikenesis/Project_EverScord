using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class NML2_Controller : NController
{
    protected override void Setup()
    {
        runState = gameObject.AddComponent<NML2_RunState>();
        attackState1 = gameObject.AddComponent<NML2_AttackState1>();
        attackState2 = gameObject.AddComponent<NML2_AttackState2>();
        waitState = gameObject.AddComponent<NML2_WaitState>();
        stunState = gameObject.AddComponent<NML2_StunState>();
        deathState = gameObject.AddComponent<NML2_DeathState>();
    }
}
