using Photon.Pun;
using System.Collections;
using UnityEngine;
using EverScord.Effects;
using EverScord;
using System;

public class BossSpawner : MonoBehaviour
{
    private GameObject mo;
    private PhotonView photonView;

    public static Action OnSpawnBoss = delegate { };

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    void Start()
    {
        LevelControl.OnProgressUpdated += ProgressCheck;
    }

    private void ProgressCheck(float currentProgress)
    {
        if(currentProgress == 1)
        {
            Spawn();
        }
    }

    private void Spawn()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        OnSpawnBoss?.Invoke();

        GameObject bossPrefab = ResourceManager.Instance.GetAsset<GameObject>(AssetReferenceManager.Boss_ID);
        mo = Instantiate(bossPrefab, transform.position, Quaternion.identity);
        mo.SetActive(true);

        PhotonView view = mo.GetComponent<PhotonView>();

        if (PhotonNetwork.AllocateViewID(view))
        {
            photonView.RPC(nameof(SyncSpawn), RpcTarget.Others, view.ViewID);
        }

        LevelControl.OnProgressUpdated -= ProgressCheck;
        gameObject.SetActive(false);
    }

    [PunRPC]
    private void SyncSpawn(int viewID)
    {
        if (PhotonNetwork.IsMasterClient)
            return;

        GameObject bossPrefab = ResourceManager.Instance.GetAsset<GameObject>(AssetReferenceManager.Boss_ID);
        mo = Instantiate(bossPrefab, transform.position, Quaternion.identity);
        mo.SetActive(true);

        PhotonView view = mo.GetComponent<PhotonView>();
        view.ViewID = viewID;

        LevelControl.OnProgressUpdated -= ProgressCheck;
        gameObject.SetActive(false);
    }
}
