using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NML1_WaitState : NWaitState
{
    protected override void Setup()
    {
        monsterController = GetComponent<NML1_Controller>();
    }
}
