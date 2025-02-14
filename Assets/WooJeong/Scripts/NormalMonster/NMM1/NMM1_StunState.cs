using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NMM1_StunState : NStunState
{
    protected override void Setup()
    {
        monsterController = GetComponent<NMM1_Controller>();
    }
}
