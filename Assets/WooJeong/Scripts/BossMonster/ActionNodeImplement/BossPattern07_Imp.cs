using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPattern07_Imp : AttackNodeImplement
{
    private float projectileSpeed = 20f;
    private float damage;
    private void Start()
    {
        damage = bossRPC.BossMonsterData.SkillDatas[6].MaxHpBasedDamage;
    }

    protected override void Awake()
    {
        base.Awake();
        attackableHP = 50;
    }

    protected override IEnumerator Act()
    {
        bossRPC.PlayAnimation("Shoot");
        yield return new WaitForSeconds(0.5f);
        Vector3[] dirs = { transform.forward, transform.right, -transform.forward, -transform.right };

        for (int i = 0; i < 7; i++)
        {
            foreach (Vector3 dir in dirs)
            {
                Vector3 direction = Quaternion.AngleAxis(-30, Vector3.up) * dir;
                Vector3 direction2 = Quaternion.AngleAxis(30, Vector3.up) * dir;
                Vector3 pos = transform.position + dir * 2;
                bossRPC.FireBossProjectile(pos, dir, damage, projectileSpeed);
                bossRPC.FireBossProjectile(pos, direction, damage, projectileSpeed);
                bossRPC.FireBossProjectile(pos, direction2, damage, projectileSpeed);
            }

            yield return new WaitForSeconds(0.14f);
        }
        yield return new WaitForSeconds(0.5f);
        bossRPC.PlayAnimation("Idle");

        isEnd = true;
        action = null;
    }

}
