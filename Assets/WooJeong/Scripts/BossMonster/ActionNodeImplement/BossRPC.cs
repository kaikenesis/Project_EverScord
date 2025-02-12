using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRPC : MonoBehaviour
{
    private PhotonView photonView;
    private Animator animator;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        animator = GetComponent<Animator>();
    }

    public void PlayAnimation(string animationName)
    {
        photonView.RPC("SyncBossAnimation", RpcTarget.All, animationName);
    }

    [PunRPC]
    public void SyncBossAnimation(string animationName)
    {
        if (animator == null || photonView == null)
        {
            photonView = GetComponent<PhotonView>();
            animator = GetComponent<Animator>();
        }
        animator.CrossFade(animationName, 0.3f, -1, 0);
    }

    public void FireBossProjectile(Vector3 position, Vector3 direction, float projectileSpeed)
    {
        photonView.RPC("SyncBossProjectile", RpcTarget.All, position, direction, projectileSpeed);
    }

    [PunRPC]
    public void SyncBossProjectile(Vector3 position, Vector3 direction, float projectileSpeed)
    {
        GameObject go = ResourceManager.Instance.GetFromPool("BossProjectile", direction, Quaternion.identity);
        BossProjectile bp = go.GetComponent<BossProjectile>();
        bp.Setup(position, direction, projectileSpeed);
    }
}
