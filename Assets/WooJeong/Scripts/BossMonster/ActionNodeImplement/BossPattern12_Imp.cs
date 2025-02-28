using EverScord;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPattern12_Imp : ActionNodeImplement
{
    private float attackDamage = 10;
    private float attackWidth = 3;

    protected override IEnumerator Act()
    {
        Debug.Log("Attack 12 Start");
        for (int i = 0; i < 3; i++)
        {
            bossRPC.PlayAnimation("StandingAttack");
            foreach (var player in GameManager.Instance.playerPhotonViews)
            {
                bossRPC.InstantiateStoneAttack2(player.transform.position, attackWidth, 1, "StoneUp", attackDamage);
            }
            yield return new WaitForSeconds(1.5f);
            bossRPC.PlayEffect("StandingAttackEffect", transform.position + transform.forward * 3);
            yield return new WaitForSeconds(bossRPC.clipDict["StandingAttack"] - 1.5f);
        }
        isEnd = true;
        action = null;
    }

}
