using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SK_112206_RunState : NRunState
{
    protected override void Setup()
    {
        monsterController = GetComponent<SK_112206_Controller>();
    }
}   
