using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NML2_RunState : NRunState
{
    protected override void Setup()
    {
        monsterController = GetComponent<NML2_Controller>();
    }
}
