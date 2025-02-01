using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SK_112206_Controller : NController
{
    protected override void Setup()
    {
        runState = gameObject.AddComponent<SK_112206_RunState>();
        attackState1 = gameObject.AddComponent<SK_112206_AttackState1>();
        attackState2 = gameObject.AddComponent<SK_112206_AttackState2>();       
        waitState = gameObject.AddComponent<SK_112206_WaitState>();
        stunState = gameObject.AddComponent<SK_112206_StunState>();
        deathState = gameObject.AddComponent<SK_112206_DeathState>();
    }
}
