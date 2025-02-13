using EverScord.Monster;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BossRPC : MonoBehaviour, IEnemy
{
    public Dictionary<string, float> clipDict = new();
    
    [SerializeField] private BossData bossData;
    [SerializeField] private GameObject laserPoint;
    private PhotonView photonView;
    private Animator animator;
    private DecalProjector projectorCharge;
    private DecalProjector projectorQuater;
    private GameObject projectorQuaterPivot;
    private float attackRadius6 = 10;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        animator = GetComponent<Animator>();
        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            clipDict[clip.name] = clip.length;
        }
        
        SetProjectors();
    }

    private void SetProjectors()
    {
        //projector1
        projectorCharge = gameObject.AddComponent<DecalProjector>();
        projectorCharge.size = new Vector3(1, 1, 10);
        projectorCharge.pivot = new Vector3(0, 0f, 5f);
        projectorCharge.material = ResourceManager.Instance.GetAsset<Material>("DecalRedSquare");
        projectorCharge.renderingLayerMask = 2;
        projectorCharge.enabled = false;

        //projector2
        projectorQuaterPivot = new GameObject();
        projectorQuaterPivot.name = "ProjectorQuaterPivot";
        projectorQuaterPivot.transform.parent = transform;
        projectorQuaterPivot.transform.localPosition = Vector3.zero;

        GameObject projectorObj = new GameObject();
        projectorObj.transform.parent = projectorQuaterPivot.transform;
        projectorObj.transform.Rotate(new Vector3(90, 0, 0));
        projectorObj.transform.localPosition = new Vector3(5, 0, attackRadius6 / 2);
        projectorQuater = projectorObj.AddComponent<DecalProjector>();
        projectorQuater.renderingLayerMask = 2;
        projectorQuater.size = new Vector3(attackRadius6, attackRadius6, 1);
        projectorQuater.material = ResourceManager.Instance.GetAsset<Material>("DecalRedQuater");
        projectorQuater.enabled = false;

        projectorQuaterPivot.transform.Rotate(new Vector3(0, -45, 0));
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

    public virtual IEnumerator ProjectEnable(int projectorNum, float projectTime)
    {
        photonView.RPC("SyncProjectorEnable", RpcTarget.All, projectorNum);
        yield return new WaitForSeconds(projectTime);
        photonView.RPC("SyncProjectorDisable", RpcTarget.All, projectorNum);
    }

    [PunRPC]
    protected void SyncProjectorEnable(int projectorNum)
    {
        if(projectorNum == 1)
            projectorCharge.enabled = true;
        else
            projectorQuater.enabled = true;
    }

    [PunRPC]
    protected void SyncProjectorDisable(int projectorNum)
    {
        if(projectorNum == 1)
            projectorCharge.enabled = false;
        else
            projectorQuater.enabled = false;
    }

    public void DecreaseHP(float hp)
    {
        Debug.Log("Boss Hit");
        photonView.RPC("SyncBossMonsterHP", RpcTarget.All, hp);
    }

    [PunRPC]
    protected void SyncBossMonsterHP(float hp)
    {
        bossData.ReduceHp(hp);
        hp = bossData.HP;
    }

    public IEnumerator LaserEnable(float enableTime)
    {
        Debug.Log("Laser active");
        laserPoint.SetActive(true);
        yield return new WaitForSeconds(enableTime);
        laserPoint.SetActive(false);
    }
}
