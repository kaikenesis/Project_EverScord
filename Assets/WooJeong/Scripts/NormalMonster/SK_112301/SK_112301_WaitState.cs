using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SK_112301_WaitState : NWaitState
{
    protected override void Setup()
    {
        monsterController = GetComponent<SK_112301_Controller>();
    }
}
