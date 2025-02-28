using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static UnityEditor.PlayerSettings;

public class MapBlackFogSpawner : MonoBehaviour
{
    [SerializeField] private AssetReferenceGameObject fog;
    private float spawnTimer = 30f;
    private float disappearTimer = 10f;
    private bool isCreate = false;
    private float randomRange = 25f;
    private float curTime = 30;

    private PhotonView photonView;
    private Coroutine spawn;
    private List<GameObject> fogList = new();

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    private async void OnEnable()
    {
        if(!isCreate)
        {
            await ResourceManager.Instance.CreatePool(fog.AssetGUID, 2);
            isCreate = true;
        }

        spawn = StartCoroutine(Spawn());
    }

    private void OnDisable()
    {
        StopCoroutine(spawn);
        spawn = null;
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
                        photonView.RPC("SyncFog", RpcTarget.Others, ranPos);
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

                        photonView.RPC("SyncFog", RpcTarget.Others, ranPos);
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
        if (fogList == null)
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
            fogList[i].SetActive(false);
        }
    }
}
