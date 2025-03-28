using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPattern11_Imp : BossPattern08_Imp
{
    protected override void Awake()
    {
        base.Awake();
        attackCount = 60;
    }
}
