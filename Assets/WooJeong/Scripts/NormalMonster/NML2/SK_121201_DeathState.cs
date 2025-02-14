using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SK_121201_DeathState : NDeathState
{
    protected override void Setup()
    {
        monsterController = GetComponent<SK_121201_Controller>();
    }
}
