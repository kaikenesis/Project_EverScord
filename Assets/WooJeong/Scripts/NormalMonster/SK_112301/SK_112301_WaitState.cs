using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SK_112301_WaitState : NWaitState
{
    private float attackRangeZ2;
    protected override void Setup()
    {
        monsterController = GetComponent<SK_112301_Controller>();
        var temp = (SK_112301_Controller)monsterController;
        attackRangeZ2 = temp.AttackRangeZ2;
    }
}
