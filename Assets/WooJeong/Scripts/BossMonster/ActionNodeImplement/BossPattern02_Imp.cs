using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPattern02_Imp : ActionNodeImplement
{
    private float projectileSpeed = 10f;

    protected override IEnumerator Act()
    {
        bossRPC.PlayAnimation("Shoot");
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < 7; i++)
        {
            Vector3 direction = Quaternion.AngleAxis(-30, Vector3.up) * transform.forward;
            Vector3 direction2 = Quaternion.AngleAxis(30, Vector3.up) * transform.forward;
            Vector3 pos = transform.position + transform.forward * 2;
            bossRPC.FireBossProjectile(pos, direction, projectileSpeed);
            bossRPC.FireBossProjectile(pos, transform.forward, projectileSpeed);
            bossRPC.FireBossProjectile(pos, direction2, projectileSpeed);

            yield return new WaitForSeconds(0.14f);
        }
        yield return new WaitForSeconds(0.5f);
        bossRPC.PlayAnimation("Idle");

        isEnd = true;
    }
}
