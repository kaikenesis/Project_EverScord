using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class BossSpawner : MonoBehaviour
{
    [SerializeField] private float spawnTimer = 0f;

    private string assetName = "Boss";
    private float curTime = 0;
    private int spawnCount = 0;

    private object data;
    private GameObject mo;
    private PhotonView photonView;

    private async void Awake()
    {
        photonView = GetComponent<PhotonView>();
        await ResourceManager.Instance.CreatePool(assetName, 1);
        StartCoroutine(Spawn());
    }

    private IEnumerator Spawn()
    {
        if (!PhotonNetwork.IsMasterClient)
            yield break;
        Debug.Log("[MasterCient] 몬스터 스폰 함수 실행");
        while (true)
        {
            curTime += Time.deltaTime;
            if (curTime > spawnTimer && spawnCount <= 0)
            {
                Debug.Log("스폰");
                mo = ResourceManager.Instance.GetFromPool(assetName, transform.position, Quaternion.identity);

                PhotonView view = mo.GetComponent<PhotonView>();

                if (PhotonNetwork.AllocateViewID(view))
                {
                    data = view.ViewID;
                    photonView.RPC("SyncSpawn", RpcTarget.Others, data);
                }

                spawnCount++;
                curTime = 0f;
                yield break;
            }
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    [PunRPC]
    private void SyncSpawn(int viewID)
    {
        if (PhotonNetwork.IsMasterClient)
            return;

        mo = ResourceManager.Instance.GetFromPool(assetName, transform.position, Quaternion.identity);
        PhotonView view = mo.GetPhotonView();
        view.ViewID = viewID;
        Debug.Log("[client] 몬스터 viewID = " + view.ViewID);
    }
}
