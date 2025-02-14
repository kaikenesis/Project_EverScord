using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NML1_StunState : NStunState
{
    protected override void Setup()
    {
        monsterController = GetComponent<NML1_Controller>();
    }
}
