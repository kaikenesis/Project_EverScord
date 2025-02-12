using Photon.Pun;
using System.Collections;
using UnityEngine;

public class BossPattern01_Imp : ActionNodeImplement
{
    //private float projectileSize = 1;
    private float projectileSpeed = 5f;

    protected override IEnumerator Action()
    {
        Debug.Log("Attack1 start");
        bossRPC.PlayAnimation("Shoot");
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < 7; i++)
        {
            bossRPC.FireBossProjectile(transform.position, transform.forward, projectileSpeed);
            //photonView.RPC("SyncBossProjectile", RpcTarget.All, transform.position, transform.forward, projectileSpeed);
            yield return new WaitForSeconds(0.14f);
        }
        yield return new WaitForSeconds(0.5f);

        isEnd = true;
        action = null;
        Debug.Log("Attack1 end");
    }
}
