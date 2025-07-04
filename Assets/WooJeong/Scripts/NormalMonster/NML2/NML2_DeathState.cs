using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NML2_DeathState : NDeathState
{
    protected override void Setup()
    {
        monsterController = GetComponent<NML2_Controller>();
    }

    public override void Enter()
    {
        base.Enter();
        monsterController.PlaySound("NML2_Die");
    }
}
