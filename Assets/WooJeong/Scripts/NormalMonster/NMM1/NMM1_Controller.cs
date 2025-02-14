using UnityEngine;
using UnityEngine.Rendering.Universal;

public class NMM1_Controller : NController
{
    protected override void Setup()
    {
        runState = gameObject.AddComponent<NMM1_RunState>();
        attackState1 = gameObject.AddComponent<NMM1_AttackState1>();
        attackState2 = gameObject.AddComponent<NMM1_AttackState2>();       
        waitState = gameObject.AddComponent<NMM1_WaitState>();
        stunState = gameObject.AddComponent<NMM1_StunState>();
        deathState = gameObject.AddComponent<NMM1_DeathState>();
    }
}
