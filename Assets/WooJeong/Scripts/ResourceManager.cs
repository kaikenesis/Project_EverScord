using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ResourceManager : Singleton<ResourceManager>, IOnEventCallback
{
    private AsyncOperationHandle<GameObject> loadOpHandle;
    private GameObject prefab;

    private const byte SpawnEventCode = 1; // 이벤트 코드 상수화

    public IEnumerator LoadAsset(string address, Vector3 position)
    {
        if (!PhotonNetwork.IsMasterClient)
            yield break;

        yield return loadOpHandle = Addressables.LoadAssetAsync<GameObject>(address);
        prefab = Instantiate(loadOpHandle.Result, position, Quaternion.identity);
        PhotonView photonView = prefab.GetComponent<PhotonView>();

        if (PhotonNetwork.AllocateViewID(photonView))
        {
            object[] data = new object[]
            {
                prefab.transform.position, prefab.transform.rotation, photonView.ViewID, address
            };

            RaiseEventOptions raiseEventOptions = new()
            {
                Receivers = ReceiverGroup.Others, // 다른 클라이언트에게만 전송
                CachingOption = EventCaching.AddToRoomCache // 새로 입장한 플레이어도 받을 수 있도록 설정
            };

            SendOptions sendOptions = new()
            {
                Reliability = true
            };

            PhotonNetwork.RaiseEvent(SpawnEventCode, data, raiseEventOptions, sendOptions);
        }
        else
        {
            Debug.LogError("Failed to allocate a ViewId.");
            Destroy(prefab);
        }
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == SpawnEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;

            // Addressables에서 동일한 프리팹을 로드하여 생성
            Addressables.LoadAssetAsync<GameObject>(data[3]).Completed += handle =>
            {
                GameObject gameObject = Instantiate(handle.Result, (Vector3)data[0], (Quaternion)data[1]);
                PhotonView photonView = gameObject.GetComponent<PhotonView>();
                photonView.ViewID = (int)data[2];
            };
        }
    }

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }
}
