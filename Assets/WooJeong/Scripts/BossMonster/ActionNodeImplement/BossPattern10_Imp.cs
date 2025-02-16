using EverScord;
using System.Collections;
using UnityEngine;

public class BossPattern10_Imp : ActionNodeImplement
{
    private float attackDamage = 10;
    private float attackWidth = 3;

    protected override IEnumerator Act()
    {
        for (int i = 0; i < 3; i++)
        {
            bossRPC.PlayAnimation("StandingAttack");
            foreach (var player in GameManager.Instance.playerPhotonViews)
            {
                bossRPC.InstantiateStoneAttack(player.transform.position, attackWidth, 1, "StoneUp", attackDamage);
            }
            yield return new WaitForSeconds(bossRPC.clipDict["StandingAttack"]);
        }
        isEnd = true;
        action = null;
    }

}
