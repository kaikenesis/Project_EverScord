using EverScord;
using System.Collections;
using EverScord.Character;
using UnityEngine;

public class BossPattern12_Imp : ActionNodeImplement
{
    private float damage;
    private float attackWidth = 3;

    private void Start()
    {
        damage = bossRPC.BossMonsterData.SkillDatas[10].SkillDamage;
    }

    protected override IEnumerator Act()
    {
        StartCoroutine(nameof(CheckDeath));
        for (int i = 0; i < 3; i++)
        {
            bossRPC.PlayAnimation("StandingAttack");
            foreach (CharacterControl player in GameManager.Instance.PlayerDict.Values)
            {
                bossRPC.InstantiateStoneAttack2(player.PlayerTransform.position, attackWidth, 1, "StoneUp", damage);
            }
            yield return new WaitForSeconds(1.5f);
            bossRPC.PlayEffect("StandingAttackEffect", transform.position + transform.forward * 3);
            bossRPC.PlaySound("BossPattern04");
            yield return new WaitForSeconds(bossRPC.clipDict["StandingAttack"] - 1.5f);
        }
        isEnd = true;
        action = null;
    }

}
