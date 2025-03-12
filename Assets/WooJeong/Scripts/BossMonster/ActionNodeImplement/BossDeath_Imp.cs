using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDeath_Imp : ActionNodeImplement
{
    protected override IEnumerator Act()
    {
        if (bossRPC.Phase == 1)
        {
            bossRPC.PlayAnimation("Roar");
            yield return new WaitForSeconds(bossRPC.clipDict["Roar"]);
            bossRPC.PhaseUp();
            isEnd = true;
            action = null;
            yield break;
        }
        bossRPC.PlayAnimation("Die");
        yield return new WaitForSeconds(bossRPC.clipDict["Die"]);
        isEnd = true;
        action = null;
        bossRPC.Death();
    }
}
