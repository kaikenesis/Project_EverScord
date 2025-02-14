using Photon.Pun;
using System.Collections;
using UnityEngine;

public class BossPattern01_Imp : ActionNodeImplement
{
    //private float projectileSize = 1;
    private float projectileSpeed = 20f;

    protected override IEnumerator Act()
    {
        bossRPC.PlayAnimation("Shoot");
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < 7; i++)
        {
            Vector3 pos = transform.position + transform.forward * 2;

            bossRPC.FireBossProjectile(pos, transform.forward, projectileSpeed);
            yield return new WaitForSeconds(0.14f);
        }
        yield return new WaitForSeconds(0.5f);
        bossRPC.PlayAnimation("Idle");

        isEnd = true;
    }
}
