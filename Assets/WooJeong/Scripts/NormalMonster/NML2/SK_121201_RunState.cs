using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SK_121201_RunState : NRunState
{
    protected override void Setup()
    {
        monsterController = GetComponent<SK_121201_Controller>();
    }
}
