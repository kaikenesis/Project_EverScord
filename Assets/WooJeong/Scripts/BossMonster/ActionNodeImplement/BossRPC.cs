using EverScord.Character;
using EverScord.Monster;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    private DecalProjector projectorP7_Danger;
    private DecalProjector projectorP7_Safe;
    private GameObject projectorQuaterPivot;
    private GameObject projectorP7_GObject;
    private float attackRadius6 = 10;
    private float attackRadius7 = 100;
    private float safeRadius7 = 7.5f;

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
        // projector for pattern4
        projectorCharge = gameObject.AddComponent<DecalProjector>();
        projectorCharge.size = new Vector3(1, 1, 10);
        projectorCharge.pivot = new Vector3(0, 0f, 5f);
        projectorCharge.material = ResourceManager.Instance.GetAsset<Material>("DecalRedSquare");
        projectorCharge.renderingLayerMask = 2;
        projectorCharge.enabled = false;

        // projector for pattern5
        projectorQuaterPivot = new GameObject();
        projectorQuaterPivot.name = "ProjectorQuaterPivot";
        projectorQuaterPivot.transform.parent = transform;
        projectorQuaterPivot.transform.localPosition = Vector3.zero;

        GameObject projectorObj = new GameObject();
        projectorObj.name = "projectorQuater";
        projectorObj.transform.parent = projectorQuaterPivot.transform;
        projectorObj.transform.Rotate(new Vector3(90, 0, 0));
        projectorObj.transform.localPosition = new Vector3(5, 0, attackRadius6 / 2);
        projectorQuater = projectorObj.AddComponent<DecalProjector>();
        projectorQuater.renderingLayerMask = 2;
        projectorQuater.size = new Vector3(attackRadius6, attackRadius6, 1);
        projectorQuater.material = ResourceManager.Instance.GetAsset<Material>("DecalRedQuater");
        projectorQuater.enabled = false;

        projectorQuaterPivot.transform.Rotate(new Vector3(0, -45, 0));

        // projector for pattern 7
        GameObject projectorForPatter7 = new GameObject();
        projectorForPatter7.transform.parent = transform;
        projectorForPatter7.transform.localPosition = Vector3.zero;
        projectorForPatter7.name = "projectorForPatter7";

        projectorP7_GObject = new GameObject();
        projectorP7_GObject.transform.parent = projectorForPatter7.transform;
        projectorP7_GObject.transform.localPosition = Vector3.zero;
        projectorP7_GObject.name = "projectorP7_SafeGObject";

        projectorP7_Danger = projectorForPatter7.AddComponent<DecalProjector>();
        projectorP7_Danger.renderingLayerMask = 2;
        projectorP7_Danger.size = new Vector3(attackRadius7, attackRadius7, 1);
        projectorP7_Danger.material = ResourceManager.Instance.GetAsset<Material>("DecalRedCircle");
        projectorP7_Danger.enabled = false;

        projectorP7_Safe = projectorP7_GObject.AddComponent<DecalProjector>();
        projectorP7_Safe.renderingLayerMask = 2;
        projectorP7_Safe.size = new Vector3(safeRadius7, safeRadius7, 1);
        projectorP7_Safe.material = ResourceManager.Instance.GetAsset<Material>("DecalGreenCircle");
        projectorP7_Safe.enabled = false;

        projectorForPatter7.transform.Rotate(new Vector3(90, 0, 0));
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
        GameObject go = ResourceManager.Instance.GetFromPool("BossProjectile", direction, Quaternion.identity);
        PhotonView view = go.GetComponent<PhotonView>();
        if(view.ViewID == 0)
        {
            if (PhotonNetwork.AllocateViewID(view))
            {
                int viewID = view.ViewID;
                photonView.RPC("SyncBossProjectile", RpcTarget.Others, position, direction, projectileSpeed, viewID);
            }
        }
        else
        {
            photonView.RPC("SyncBossProjectile", RpcTarget.Others, position, direction, projectileSpeed, view.ViewID);
        }
        BossProjectile bp = go.GetComponent<BossProjectile>();
        bp.Setup(position, direction, projectileSpeed);
    }

    [PunRPC]
    public void SyncBossProjectile(Vector3 position, Vector3 direction, float projectileSpeed, int viewID)
    {
        GameObject go = ResourceManager.Instance.GetFromPool("BossProjectile", direction, Quaternion.identity);
        PhotonView view = go.GetComponent<PhotonView>();
        if(view.ViewID == 0)
            view.ViewID = viewID;
        BossProjectile bp = go.GetComponent<BossProjectile>();
        bp.Setup(position, direction, projectileSpeed);
    }

    public IEnumerator ProjectEnable(int patternNum, float projectTime)
    {
        photonView.RPC("SyncProjectorEnable", RpcTarget.All, patternNum);
        yield return new WaitForSeconds(projectTime);
        photonView.RPC("SyncProjectorDisable", RpcTarget.All, patternNum);
    }

    [PunRPC]
    protected void SyncProjectorEnable(int patternNum)
    {
        switch (patternNum)
        {
            case 4:
                {
                    projectorCharge.enabled = true;
                    break;
                }
            case 5:
                {
                    projectorQuater.enabled = true;
                    break ;
                }
            case 7:
                {
                    projectorP7_Danger.enabled = true;
                    projectorP7_Safe.enabled = true;
                    break;
                }
        }
    }

    [PunRPC]
    protected void SyncProjectorDisable(int patternNum)
    {
        switch (patternNum)
        {
            case 4:
                {
                    projectorCharge.enabled = false;
                    break;
                }
            case 5:
                {
                    projectorQuater.enabled = false;
                    break;
                }
            case 7:
                {
                    projectorP7_Danger.enabled = false;
                    projectorP7_Safe.enabled = false;
                    break;
                }
        }
    }

    public void MoveProjectorP7_Safe(Vector3 pos)
    {
        photonView.RPC("SyncProjectorP7_SafePosition", RpcTarget.All, pos);
    }

    [PunRPC]
    private void SyncProjectorP7_SafePosition(Vector3 pos)
    {
        projectorP7_Safe.transform.position = pos;
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

    public void StunMonster(float stunTime)
    {
        return;
    }

    public void InstantiateStoneAttack(Vector3 pos, float width, float projectTime, string effectAddressableKey)
    {
        photonView.RPC("SyncStoneAttack", RpcTarget.All, pos, width, projectTime, effectAddressableKey);
    }

    public void InstantiateStoneAttack(Vector3 pos, float width, float projectTime, string effectAddressableKey, bool isPhase2)
    {
        photonView.RPC("SyncStoneAttack2", RpcTarget.All, pos, width, projectTime, effectAddressableKey, isPhase2);
    }

    [PunRPC]
    protected void SyncStoneAttack(Vector3 pos, float width, float projectTime, string addressableKey)
    {
        GameObject go = ResourceManager.Instance.GetFromPool("MonsterAttack", pos, Quaternion.identity);
        MonsterAttack ma = go.GetComponent<MonsterAttack>();
        ma.Setup(width, projectTime, addressableKey);
    }

    [PunRPC]
    protected void SyncStoneAttack2(Vector3 pos, float width, float projectTime, string addressableKey, bool isPhase2)
    {
        GameObject go = ResourceManager.Instance.GetFromPool("MonsterAttack", pos, Quaternion.identity);
        MonsterAttack ma = go.GetComponent<MonsterAttack>();
        ma.Setup(width, projectTime, addressableKey, isPhase2);
    }
}
