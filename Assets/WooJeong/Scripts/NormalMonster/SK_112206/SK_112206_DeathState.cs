using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SK_112206_DeathState : NDeathState
{
    protected override void Setup()
    {
        monsterController = GetComponent<SK_112206_Controller>();
    }
}
