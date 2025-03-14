using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPattern02_Imp : ActionNodeImplement
{
    private float projectileSpeed = 20f;
    private float damage;

    private void Start()
    {
        damage = bossRPC.BossMonsterData.SkillDatas[1].MaxHpBasedDamage;
    }

    protected override IEnumerator Act()
    {
        Debug.Log("Attack2");
        bossRPC.PlayAnimation("Shoot");
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < 7; i++)
        {
            Vector3 direction = Quaternion.AngleAxis(-30, Vector3.up) * transform.forward;
            Vector3 direction2 = Quaternion.AngleAxis(30, Vector3.up) * transform.forward;
            Vector3 pos = transform.position + transform.forward * 2;
            bossRPC.FireBossProjectile(pos, direction, damage, projectileSpeed);
            bossRPC.FireBossProjectile(pos, transform.forward, damage, projectileSpeed);
            bossRPC.FireBossProjectile(pos, direction2, damage, projectileSpeed);

            yield return new WaitForSeconds(0.14f);
        }
        yield return new WaitForSeconds(0.5f);
        bossRPC.PlayAnimation("Idle");

        isEnd = true;
        action = null;
    }
}
