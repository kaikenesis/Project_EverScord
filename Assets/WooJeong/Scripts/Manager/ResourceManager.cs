using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UIElements;

public class ResourceManager : Singleton<ResourceManager>, IOnEventCallback
{
    private const byte SpawnMonsterEventCode = 1;
    private const byte SpawnCompleteEventCode = 2;
    private const byte DestroyMonsterEventCode = 3;
    private const byte InstantiateEventCode = 4;
    
    private Dictionary<int, int> viewIDCount = new();
    private Dictionary<int, GameObject> prefabDict = new();
    private List<GameObject> prefabList = new();

    public IEnumerator SpawnMonster(string address, Vector3 position)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            var loadOpHandle = Addressables.InstantiateAsync(address, position, Quaternion.identity);
            yield return loadOpHandle;

            if (loadOpHandle.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject prefab = loadOpHandle.Result;
                PhotonView photonView = prefab.GetComponent<PhotonView>();
                
                if (PhotonNetwork.AllocateViewID(photonView))
                {
                    if (!viewIDCount.ContainsKey(photonView.ViewID))
                    {
                        viewIDCount[photonView.ViewID] = 0;
                        prefabDict[photonView.ViewID] = prefab;
                    }
                    
                    object[] data = new object[]
                    {
                        prefab.transform.position, prefab.transform.rotation, photonView.ViewID, address
                    };

                    RaiseEventOptions raiseEventOptions = new()
                    {
                        Receivers = ReceiverGroup.Others,
                        CachingOption = EventCaching.AddToRoomCache
                    };

                    SendOptions sendOptions = new() { Reliability = true };

                    PhotonNetwork.RaiseEvent(SpawnMonsterEventCode, data, raiseEventOptions, sendOptions);
                    PhotonNetwork.RaiseEvent(SpawnCompleteEventCode, photonView.ViewID,
                        new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient },
                        new SendOptions { Reliability = true });
                }
            }
        }
    }

    public async Task<GameObject> InstantiatePrefab(string address, Vector3 position)
    {
        AsyncOperationHandle<GameObject> loadOpHandle = Addressables.InstantiateAsync(address, position, Quaternion.identity);

        await loadOpHandle.Task; // 비동기 작업이 끝날 때까지 기다림

        if (loadOpHandle.Status == AsyncOperationStatus.Succeeded)
        {
            return loadOpHandle.Result;
        }
        else
        {
            Debug.LogError($"Failed to load Addressable prefab: {address}");
            return null;
        }
    }

    public void DestroyMonster(GameObject gameObject)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            int viewID = gameObject.GetPhotonView().ViewID;
            prefabDict.Remove(viewID);
            viewIDCount.Remove(viewID);
            Destroy(gameObject);

            RaiseEventOptions raiseEventOptions = new()
            {
                Receivers = ReceiverGroup.Others,
                CachingOption = EventCaching.AddToRoomCache
            };

            SendOptions sendOptions = new() { Reliability = true };

            PhotonNetwork.RaiseEvent(DestroyMonsterEventCode, viewID, raiseEventOptions, sendOptions);
        }
    }

    public void DestroyPrefab(GameObject gameObject)
    {
        Destroy(gameObject);
    }

    public void OnEvent(EventData photonEvent)
    {
        switch (photonEvent.Code)
        {
            case SpawnMonsterEventCode:
                {
                    object[] data = (object[])photonEvent.CustomData;
                    Vector3 position = (Vector3)data[0];
                    Quaternion rotation = (Quaternion)data[1];
                    int viewID = (int)data[2];
                    string address = (string)data[3];

                    var loadOpHandle = Addressables.InstantiateAsync(address, position, rotation);

                    loadOpHandle.Completed += handle =>
                    {
                        GameObject gameObject = handle.Result;
                        PhotonView photonView = gameObject.GetComponent<PhotonView>();
                        photonView.ViewID = viewID;
                        PhotonNetwork.RegisterPhotonView(photonView);
                        prefabDict[viewID] = gameObject;

                        PhotonNetwork.RaiseEvent(SpawnCompleteEventCode, viewID,
                            new RaiseEventOptions { Receivers = ReceiverGroup.MasterClient },
                            new SendOptions { Reliability = true });
                    };
                    break;
                }

            case SpawnCompleteEventCode:
                {
                    if (PhotonNetwork.IsMasterClient)
                    {
                        int viewID = (int)photonEvent.CustomData;
                        viewIDCount[viewID]++;

                        if (viewIDCount[viewID] == PhotonNetwork.PlayerList.Length)
                        {
                            Debug.Log("[MasterClient] 모든 플레이어가 스폰 완료! FSM 시작");
                            NController nController = prefabDict[viewID].GetComponent<NController>();
                            nController.StartFSM();
                        }
                    }
                    break;
                }

            case DestroyMonsterEventCode:
                {
                    int viewID = (int)photonEvent.CustomData;
                    GameObject gameObject = prefabDict[viewID];
                    prefabDict.Remove(viewID);
                    Destroy(gameObject);
                    break;
                }

            case InstantiateEventCode:
                {
                    object[] data = (object[])photonEvent.CustomData;
                    string address = (string)data[0];
                    Vector3 position = (Vector3)data[1];
                    var loadOpHandle = Addressables.InstantiateAsync(address, position, Quaternion.identity);
                    break;
                }
        }
    }
}
