using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NML1_RunState : NRunState
{
    private float chargeRange;

    protected override void Setup()
    {
        monsterController = GetComponent<NML1_Controller>();
    }

    private void Start()
    {
        var temp = monsterController as NML1_Controller;
        chargeRange = temp.ChargeRange;
    }
}
