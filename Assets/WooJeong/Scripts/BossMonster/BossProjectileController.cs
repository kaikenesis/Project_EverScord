using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossProjectileController : MonoBehaviour
{
    private Dictionary<int, BossProjectile> projectileDict = new Dictionary<int, BossProjectile>();
    private int idNum = 0;
    private PhotonView photonView;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    private void Update()
    {
        if(!photonView.IsMine)
            return;

        foreach(var projectile in projectileDict.Values)
        {
            if(projectile.IsDestroyed)
            {
                ReturnProjectile(projectile.ID);
            }
        }
    }

    public void AddDict(int id, BossProjectile projectile)
    {
        projectileDict[id] = projectile;
    }

    public int GetIDNum()
    {
        return ++idNum;
    }

    public BossProjectile GetProjectile(int id)
    {
        return projectileDict[id];
    }

    public void DeleteProjectile(int id)
    {
        projectileDict.Remove(id);
    }

    private void ReturnProjectile(int id)
    {
        photonView.RPC("SyncProjectileReturn", RpcTarget.All, id);
    }

    [PunRPC]
    public void SyncProjectileReturn(int id)
    {
        ResourceManager.Instance.ReturnToPool(projectileDict[id].gameObject, "BossProjectile");
        projectileDict[id].SetIsDestroyed(false);
    }
}