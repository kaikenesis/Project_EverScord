using EverScord;
using EverScord.Character;
using System.Collections;
using UnityEngine;

public class BossPattern09_Imp : ActionNodeImplement
{
    private float damage;
    private float attackWidth = 3;

    protected override void Awake()
    {
        base.Awake();
        damage = bossRPC.BossMonsterData.SkillDatas[8].SkillDamage;
    }

    protected override IEnumerator Act()
    {
        StartCoroutine(nameof(CheckDeath));
        for (int i = 0; i < 3; i++)
        {
            bossRPC.PlayAnimation("StandingAttack");
            foreach (CharacterControl player in GameManager.Instance.PlayerDict.Values)
            {
                bossRPC.InstantiateStoneAttack(player.PlayerTransform.position, attackWidth, 1, "StoneUp", damage, "BossStoneUpSound");
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
