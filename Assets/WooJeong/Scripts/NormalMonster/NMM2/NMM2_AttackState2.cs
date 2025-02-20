using System.Collections;
using UnityEngine;
using Photon.Pun;
using EverScord;

public class NMM2_AttackState2 : NAttackState
{
    protected override IEnumerator Attack()
    {
        monsterController.PlayAnimation("Attack2");
        float time = monsterController.clipDict["Attack2"];
        yield return new WaitForSeconds(time/2);
        Fire();
        yield return new WaitForSeconds(time/2);

        StartCoroutine(monsterController.CoolDown2());
        attack = null;
        Exit();
    }

    protected override void Setup()
    {
        monsterController = GetComponent<NMM2_Controller>();
    }

    private void Fire()
    {
        Vector3 position = transform.position + transform.forward * 2;
        float projectileSpeed = 20;
        GameObject go = ResourceManager.Instance.GetFromPool("MonsterProjectile", position, Quaternion.identity);
        MonsterProjectile mp = go.GetComponent<MonsterProjectile>();

        int id;
        if (mp.ID == -1)
        {
            id = GameManager.Instance.ProjectileController.GetIDNum();
            GameManager.Instance.ProjectileController.AddDict(id, mp);
        }
        else
            id = mp.ID;

        mp.Setup("NMM2_Projectile", id, position, transform.forward, projectileSpeed);
        monsterController.PhotonView.RPC("SyncProjectileNMM2", RpcTarget.Others, id, position, transform.forward, projectileSpeed);
    }

    [PunRPC]
    private void SyncProjectileNMM2(int id, Vector3 position, Vector3 direction, float projectileSpeed)
    {
        GameObject go = ResourceManager.Instance.GetFromPool("MonsterProjectile", position, Quaternion.identity);
        MonsterProjectile mp = go.GetComponent<MonsterProjectile>();
        GameManager.Instance.ProjectileController.AddDict(id, mp);
        mp.Setup("NMM2_Projectile", id, position, transform.forward, projectileSpeed);
    }
}
