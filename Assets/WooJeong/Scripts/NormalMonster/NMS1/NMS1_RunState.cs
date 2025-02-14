using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NMS1_RunState : NRunState
{
    protected override void Setup()
    {
        monsterController = GetComponent<NMS1_Controller>();
    }
}
