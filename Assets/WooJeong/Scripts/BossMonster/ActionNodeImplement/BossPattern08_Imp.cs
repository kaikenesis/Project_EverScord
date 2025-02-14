using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPattern08_Imp : ActionNodeImplement
{
    private float projectileSpeed = 20f;

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
                bossRPC.FireBossProjectile(pos, dir, projectileSpeed);
                bossRPC.FireBossProjectile(pos, direction, projectileSpeed);
                bossRPC.FireBossProjectile(pos, direction2, projectileSpeed);
            }

            yield return new WaitForSeconds(0.14f);
        }
        yield return new WaitForSeconds(0.5f);
        bossRPC.PlayAnimation("Idle");

        isEnd = true;
    }

}
