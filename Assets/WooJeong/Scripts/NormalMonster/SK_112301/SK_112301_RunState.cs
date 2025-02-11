using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SK_112301_RunState : NRunState
{
    private float chargeRange;

    protected override void Setup()
    {
        monsterController = GetComponent<SK_112301_Controller>();
    }

    private void Start()
    {
        var temp = monsterController as SK_112301_Controller;
        chargeRange = temp.ChargeRange;
    }
}
