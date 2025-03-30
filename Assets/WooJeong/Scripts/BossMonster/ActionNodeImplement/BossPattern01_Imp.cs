using Photon.Pun;
using System.Collections;
using UnityEngine;

public class BossPattern01_Imp : ActionNodeImplement
{
    //private float projectileSize = 1;
    private float projectileSpeed = 20f;
    private float damage;

    private void Start()
    {
        damage = bossRPC.BossMonsterData.SkillDatas[0].MaxHpBasedDamage;
    }

    protected override IEnumerator Act()
    {
        StartCoroutine(nameof(CheckDeath));
        bossRPC.PlayAnimation("Shoot");
        yield return new WaitForSeconds(0.5f);
        bossRPC.PlaySound("BossPatternShoot");
        for (int i = 0; i < 7; i++)
        {
            Vector3 pos = transform.position + transform.forward * 2;

            bossRPC.FireBossProjectile(pos, transform.forward, damage, projectileSpeed);
            yield return new WaitForSeconds(0.14f);
        }
        yield return new WaitForSeconds(0.5f);
        bossRPC.PlayAnimation("Idle");

        isEnd = true;
        action = null;
    }
}
