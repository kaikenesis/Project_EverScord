using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NML2_StunState : NStunState
{
    protected override void Setup()
    {
        monsterController = GetComponent<NML2_Controller>();
    }
}
