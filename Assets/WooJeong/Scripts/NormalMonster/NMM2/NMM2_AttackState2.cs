using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NMM2_AttackState2 : NAttackState
{
    protected override IEnumerator Attack()
    {
        throw new System.NotImplementedException();
    }

    protected override void Setup()
    {
        monsterController = GetComponent<NMM2_Controller>();
    }
}
