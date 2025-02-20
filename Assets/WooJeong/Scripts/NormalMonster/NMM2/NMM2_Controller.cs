using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NMM2_Controller : NController
{
    protected override void Setup()
    {
        Projector1.material = ResourceManager.Instance.GetAsset<Material>("DecalRedCircle");

        runState = gameObject.AddComponent<NMM2_RunState>();
        attackState1 = gameObject.AddComponent<NMM2_AttackState1>();
        attackState2 = gameObject.AddComponent<NMM2_AttackState2>();
        waitState = gameObject.AddComponent<NMM2_WaitState>();
        stunState = gameObject.AddComponent<NMM2_StunState>();
        deathState = gameObject.AddComponent<NMM2_DeathState>();
    }
}
