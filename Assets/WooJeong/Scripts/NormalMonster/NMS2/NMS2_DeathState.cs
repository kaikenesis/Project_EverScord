using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NMS2_DeathState : NDeathState
{
    protected override void Setup()
    {
        monsterController = GetComponent<NMS2_Controller>();
    }
}
