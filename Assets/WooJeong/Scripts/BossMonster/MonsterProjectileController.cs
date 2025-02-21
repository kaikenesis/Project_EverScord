using EverScord;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterProjectileController : MonoBehaviour
{
    private Dictionary<int, MonsterProjectile> projectileDict = new Dictionary<int, MonsterProjectile>();
    private int idNum = 0;
    private PhotonView photonView;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        GameManager.Instance.InitControl(this);
    }

    private void Update()
    {
        if(!PhotonNetwork.IsMasterClient)
            return;

        foreach(var projectile in projectileDict.Values)
        {
            if(projectile.IsDestroyed)
            {
                ReturnProjectile(projectile.ID);
            }
        }
    }

    public void AddDict(int id, MonsterProjectile projectile)
    {
        projectileDict[id] = projectile;
    }

    public int GetIDNum()
    {
        return ++idNum;
    }

    public MonsterProjectile GetProjectile(int id)
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
        projectileDict[id].SetIsDestroyed(false);
        ResourceManager.Instance.ReturnToPool(projectileDict[id].ProjectileEffect, projectileDict[id].ProjectileName);
        ResourceManager.Instance.ReturnToPool(projectileDict[id].gameObject, "MonsterProjectile");
    }
}