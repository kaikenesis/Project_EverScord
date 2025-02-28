using EverScord;
using EverScord.Character;
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
            foreach (CharacterControl player in GameManager.Instance.PlayerDict.Values)
            {
                bossRPC.InstantiateStoneAttack(player.PlayerTransform.position, attackWidth, 1, "StoneUp", attackDamage);
            }
            yield return new WaitForSeconds(bossRPC.clipDict["StandingAttack"]);
        }
        isEnd = true;
        action = null;
    }

}
