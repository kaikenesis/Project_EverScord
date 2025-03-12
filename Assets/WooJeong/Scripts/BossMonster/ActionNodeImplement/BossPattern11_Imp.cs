using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPattern11_Imp : BossPattern09_Imp
{
    protected override void Awake()
    {
        base.Awake();
        attackCount = 60;
        failurePhase = 1;
    }
}
