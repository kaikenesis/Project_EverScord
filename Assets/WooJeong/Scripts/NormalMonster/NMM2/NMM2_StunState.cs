using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NMM2_StunState : NStunState
{
    protected override void Setup()
    {
        monsterController = GetComponent<NMM2_Controller>();
    }
}
