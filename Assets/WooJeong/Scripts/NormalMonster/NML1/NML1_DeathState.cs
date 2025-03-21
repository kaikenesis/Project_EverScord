using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NML1_DeathState : NDeathState
{
    protected override void Setup()
    {
        monsterController = GetComponent<NML1_Controller>();
    }

    public override void Enter()
    {
        base.Enter();
        monsterController.PlaySound("NML1_Die");
    }
}
