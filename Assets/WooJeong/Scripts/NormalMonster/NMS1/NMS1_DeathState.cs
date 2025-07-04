using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NMS1_DeathState : NDeathState
{
    protected override void Setup()
    {
        monsterController = GetComponent<NMS1_Controller>();
    }

    public override void Enter()
    {
        base.Enter();
        monsterController.PlaySound("NMS1_Die");
    }
}
