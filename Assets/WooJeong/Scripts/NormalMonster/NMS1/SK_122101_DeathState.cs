using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SK_122101_DeathState : NDeathState
{
    protected override void Setup()
    {
        monsterController = GetComponent<SK_122101_Controller>();
    }
}
