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
            bossRPC.PlaySound("BossRoarSound");
            yield return new WaitForSeconds(bossRPC.clipDict["Roar"]);
            bossRPC.StopSound("BossRoarSound");
            bossRPC.PhaseUp();
            isEnd = true;
            action = null;
            yield break;
        }
    }
}
