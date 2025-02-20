using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NMS2_WaitState : NWaitState
{
    protected override void Setup()
    {
        monsterController = GetComponent<NMS2_Controller>();
    }
}
