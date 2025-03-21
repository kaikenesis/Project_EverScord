using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NMM2_DeathState : NDeathState
{
    protected override void Setup()
    {
        monsterController = GetComponent<NMM2_Controller>();
    }

    public override void Enter()
    {
        base.Enter();
        monsterController.PlaySound("NMM2_Die");
    }
}
