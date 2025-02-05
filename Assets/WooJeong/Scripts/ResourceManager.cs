using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ResourceManager : Singleton<ResourceManager>, IOnEventCallback
{
    private const byte SpawnEventCode = 1;
    public int test = 0;

    public IEnumerator LoadAsset(string address, Vector3 position)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            AsyncOperationHandle<GameObject> loadOpHandle = Addressables.InstantiateAsync(address, position, Quaternion.identity);
            yield return loadOpHandle;

            if (loadOpHandle.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject prefab = loadOpHandle.Result;
                PhotonView photonView = prefab.GetComponent<PhotonView>();

                if (PhotonNetwork.AllocateViewID(photonView))
                {
                    object[] data = new object[]
                    {
                        prefab.transform.position, prefab.transform.rotation, photonView.ViewID, address
                    };

                    RaiseEventOptions raiseEventOptions = new()
                    {
                        Receivers = ReceiverGroup.Others,  // 다른 클라이언트에게만 전달
                        CachingOption = EventCaching.AddToRoomCache
                    };

                    SendOptions sendOptions = new()
                    {
                        Reliability = true
                    };

                    PhotonNetwork.RaiseEvent(SpawnEventCode, data, raiseEventOptions, sendOptions);
                }
            }
        }
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == SpawnEventCode)
        {
            Debug.Log("[Client] OnEvent received!");

            object[] data = (object[])photonEvent.CustomData;
            Vector3 position = (Vector3)data[0];
            Quaternion rotation = (Quaternion)data[1];
            int viewID = (int)data[2];
            string address = (string)data[3];

            Addressables.InstantiateAsync(address, position, rotation).Completed += handle =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    GameObject gameObject = handle.Result;
                    PhotonView photonView = gameObject.GetComponent<PhotonView>();
                    photonView.ViewID = viewID;
                    PhotonNetwork.RegisterPhotonView(photonView);
                }
            };
        }
    }

    private void OnEnable()
    {
        Debug.Log("[ResourceManager] OnEnable: Registering callback target.");
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        Debug.Log("[ResourceManager] OnDisable: Removing callback target.");
        PhotonNetwork.RemoveCallbackTarget(this);
    }
}
