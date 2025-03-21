using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NMM1_DeathState : NDeathState
{
    protected override void Setup()
    {
        monsterController = GetComponent<NMM1_Controller>();
    }

    public override void Enter()
    {
        base.Enter();
        monsterController.PlaySound("NMM1_Die");
    }
}
