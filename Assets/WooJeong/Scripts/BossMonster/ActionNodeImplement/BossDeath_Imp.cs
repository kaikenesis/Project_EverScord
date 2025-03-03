using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDeath_Imp : ActionNodeImplement
{
    protected override IEnumerator Act()
    {
        bossRPC.PlayAnimation("Die");
        yield return new WaitForSeconds(bossRPC.clipDict["Die"]);
        isEnd = true;
        action = null;
        bossRPC.Death();
    }
}
