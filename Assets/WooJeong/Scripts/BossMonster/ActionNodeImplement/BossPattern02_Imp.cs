using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPattern02_Imp : ActionNodeImplement
{
    private float projectileSpeed = 5f;

    protected override IEnumerator Act()
    {
        Debug.Log("Attack2 start");
        bossRPC.PlayAnimation("Shoot");
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < 7; i++)
        {
            Vector3 direction = Quaternion.AngleAxis(-30, Vector3.up) * transform.forward;
            Vector3 direction2 = Quaternion.AngleAxis(30, Vector3.up) * transform.forward;

            bossRPC.FireBossProjectile(transform.position, direction, projectileSpeed);
            bossRPC.FireBossProjectile(transform.position, transform.forward, projectileSpeed);
            bossRPC.FireBossProjectile(transform.position, direction2, projectileSpeed);

            yield return new WaitForSeconds(0.14f);
        }
        yield return new WaitForSeconds(0.5f);
        bossRPC.PlayAnimation("Idle");

        isEnd = true;
        action = null;
        Debug.Log("Attack2 end");
    }
}
