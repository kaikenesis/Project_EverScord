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

    private const byte SpawnEventCode = 1; // �̺�Ʈ �ڵ� ���ȭ

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
                Receivers = ReceiverGroup.Others, // �ٸ� Ŭ���̾�Ʈ���Ը� ����
                CachingOption = EventCaching.AddToRoomCache // ���� ������ �÷��̾ ���� �� �ֵ��� ����
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

            // Addressables���� ������ �������� �ε��Ͽ� ����
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
