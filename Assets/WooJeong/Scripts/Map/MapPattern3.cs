using EverScord;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class MapPattern3 : MonoBehaviour
{
    [SerializeField] private AssetReferenceGameObject fog;
    private float spawnTimer = 30f;
    private float disappearTimer = 10f;
    private float randomRange = 25f;
    private float curTime = 20;

    private PhotonView photonView;
    private List<GameObject> fogList = new();

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    private async void Start()
    {
        LevelControl.OnProgressUpdated += ProgressCheck;
        if (!ResourceManager.Instance.IsPoolExist(fog.AssetGUID))
            await ResourceManager.Instance.CreatePool(fog.AssetGUID, 2);

        StartCoroutine(Spawn());
    }

    protected void ProgressCheck(float currentProgress)
    {
        if (currentProgress == 1)
        {
            // 현재 진행도 체크하고 다 됐으면 죽임
            gameObject.SetActive(false);
        }
    }

    private IEnumerator Spawn()
    {
        if (!PhotonNetwork.IsMasterClient)
            yield break;
        bool isDisappear = false;
        while (true)
        {            
            curTime++;

            if (curTime > disappearTimer && isDisappear == false)
            {
                for (int i = 0; i < 2; i++)
                {
                    if(fogList.Count > 0)
                        fogList[i].SetActive(false);
                    photonView.RPC("SyncMapFogDisable", RpcTarget.Others);
                }
                isDisappear = true;
            }

            if (curTime > spawnTimer)
            {
                if(fogList.Count == 0)
                {
                    for(int i = 0; i < 2; i++)
                    {
                        float randomX = Random.Range(-randomRange, randomRange);
                        float randomZ = Random.Range(-randomRange, randomRange);
                        Vector3 ranPos = new Vector3(transform.position.x + randomX, 0, transform.position.z + randomZ);

                        GameObject mo = ResourceManager.Instance.GetFromPool(fog.AssetGUID, ranPos, Quaternion.identity);
                        Debug.Log(ranPos);
                        Debug.Log(mo);
                        fogList.Add(mo);
                        photonView.RPC("SyncMapFog", RpcTarget.Others, ranPos);
                    }                    
                }
                else
                {
                    for (int i = 0; i < 2; i++)
                    {
                        float randomX = Random.Range(-randomRange, randomRange);
                        float randomZ = Random.Range(-randomRange, randomRange);
                        Vector3 ranPos = new Vector3(transform.position.x + randomX, 0, transform.position.z + randomZ);

                        fogList[i].SetActive(true);
                        fogList[i].transform.position = ranPos;

                        photonView.RPC("SyncMapFog", RpcTarget.Others, ranPos);
                    }
                }
                curTime = 0f;
                isDisappear = false;
            }

            yield return new WaitForSeconds(1f);
        }
    }

    [PunRPC]
    private void SyncMapFog(Vector3 pos)
    {
        if (fogList.Count == 0)
        {
            for (int i = 0; i < 2; i++)
            {
                GameObject mo = ResourceManager.Instance.GetFromPool(fog.AssetGUID, pos, Quaternion.identity);
                fogList.Add(mo);
            }
        }
        else
        {
            for (int i = 0; i < 2; i++)
            {
                fogList[i].SetActive(true);
                fogList[i].transform.position = pos;
            }
        }
    }

    [PunRPC]
    private void SyncMapFogDisable()
    {
        for (int i = 0; i < 2; i++)
        {
            if (fogList.Count > 0)
                fogList[i].SetActive(false);
        }
    }
}
