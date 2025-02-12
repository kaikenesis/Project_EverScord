using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPattern05_Imp : ActionNodeImplement
{
    private float attackRadius = 10;
    protected override IEnumerator Act()
    {
        Vector3 projectorPos = transform.position + (transform.forward * Mathf.Sqrt(attackRadius * attackRadius)/2);
        bossRPC.QuaterProjectEnable(projectorPos, 1);
        yield return null;
    }
}
