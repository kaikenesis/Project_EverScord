using EverScord;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPattern12_Imp : ActionNodeImplement
{
    protected override IEnumerator Act()
    {
        for (int i = 0; i < 3; i++)
        {
            bossRPC.PlayAnimation("StandingAttack");
            foreach (var player in GameManager.Instance.playerPhotonViews)
            {
                bossRPC.InstantiateStoneAttack(player.transform.position, 5, 1, "StoneUp", true);
            }
            yield return new WaitForSeconds(bossRPC.clipDict["StandingAttack"]);
        }
        isEnd = true;
        action = null;
    }

}
