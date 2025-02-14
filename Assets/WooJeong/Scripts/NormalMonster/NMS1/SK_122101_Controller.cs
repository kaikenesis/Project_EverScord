using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SK_122101_Controller : NController
{
    protected override void Setup()
    {
        runState = gameObject.AddComponent<SK_122101_RunState>();
        attackState1 = gameObject.AddComponent<SK_122101_AttackState1>();
        attackState2 = gameObject.AddComponent<SK_122101_AttackState2>();
        waitState = gameObject.AddComponent<SK_122101_WaitState>();
        stunState = gameObject.AddComponent<SK_122101_StunState>();
        deathState = gameObject.AddComponent<SK_122101_DeathState>();
    }
}
