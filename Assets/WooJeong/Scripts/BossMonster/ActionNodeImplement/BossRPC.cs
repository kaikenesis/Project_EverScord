using EverScord.Monster;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BossRPC : MonoBehaviour, IEnemy
{
    [SerializeField] private BossData bossData;
    private PhotonView photonView;
    private Animator animator;
    private DecalProjector projectorCharge;
    private DecalProjector projectorQuater;
    private GameObject goProjectorQ;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        animator = GetComponent<Animator>();
        projectorCharge = GetComponent<DecalProjector>();
        projectorCharge.size = new Vector3(1, 1, 10);
        projectorCharge.pivot = new Vector3(0, 0f, 5f);
        goProjectorQ = new GameObject();
        goProjectorQ.name = "ProjectorQuater";
        projectorQuater = goProjectorQ.GetComponent<DecalProjector>();
        goProjectorQ.transform.parent = transform;
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

    public virtual IEnumerator ChargeProjectEnable(float projectTime)
    {
        photonView.RPC("SyncChargeProjectorEnable", RpcTarget.All);
        yield return new WaitForSeconds(projectTime);
        photonView.RPC("SyncChargeProjectorDisable", RpcTarget.All);
    }

    [PunRPC]
    protected void SyncChargeProjectorEnable()
    {
        projectorCharge.enabled = true;
    }

    [PunRPC]
    protected void SyncChargeProjectorDisable()
    {
        projectorCharge.enabled = false;
    }

    public virtual IEnumerator QuaterProjectEnable(Vector3 position, float projectTime)
    {
        photonView.RPC("SyncChargeProjectorEnable", RpcTarget.All, position);
        yield return new WaitForSeconds(projectTime);
        photonView.RPC("SyncChargeProjectorDisable", RpcTarget.All, position);
    }

    [PunRPC]
    protected void SyncQuaterProjectorEnable(Vector3 position)
    {
        goProjectorQ.transform.position = position;
        projectorQuater.enabled = true;
    }

    [PunRPC]
    protected void SyncQuaterProjectorDisable(Vector3 position)
    {
        projectorQuater.enabled = false;
    }

    public void DecreaseHP(float hp)
    {
        photonView.RPC("SyncBossMonsterHP", RpcTarget.All, hp);
    }

    [PunRPC]
    protected void SyncBossMonsterHP(float hp)
    {
        bossData.ReduceHp(hp);
    }
}
