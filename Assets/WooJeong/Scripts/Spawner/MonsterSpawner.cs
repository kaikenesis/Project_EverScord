using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField] private AssetReferenceGameObject monster;
    [SerializeField] private float spawnTimer = 0f;
    private float curTime = 0;
    private int spawnCount = 0;
    [SerializeField] private int maxSpawnCount = 5;
    private int allocateCompleteCount = 1;

    private int data;
    private GameObject mo;
    private PhotonView photonView;

    private async void Awake()
    {
        photonView = GetComponent<PhotonView>();
        await ResourceManager.Instance.CreatePool(monster.AssetGUID);
        StartCoroutine(Spawn());
    }

    private IEnumerator Spawn()
    {
        if(!PhotonNetwork.IsMasterClient)
            yield break;
        Debug.Log("[MasterCient] ���� ���� �Լ� ����");
        while(true)
        {
            curTime += Time.deltaTime;
            if (curTime > spawnTimer && spawnCount <= maxSpawnCount)
            {
                Debug.Log("����");
                mo = ResourceManager.Instance.GetFromPool(monster.AssetGUID, transform.position, Quaternion.identity);

                PhotonView view = mo.GetComponent<PhotonView>();

                if (PhotonNetwork.AllocateViewID(view))
                {
                    data = view.ViewID;
                    photonView.RPC("SyncSpawn", RpcTarget.Others, data);
                }

                NController nController = mo.GetComponent<NController>();
                nController.SetGUID(monster.AssetGUID);

                if (allocateCompleteCount == PhotonNetwork.PlayerList.Length)
                {
                    Debug.Log("[MasterClient] ��� �÷��̾ ���� �Ϸ�! FSM ����");
                    allocateCompleteCount = 1;
                    nController.StartFSM();
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

        mo = ResourceManager.Instance.GetFromPool(monster.AssetGUID, transform.position, Quaternion.identity);
        PhotonView view = mo.GetPhotonView();
        view.ViewID = viewID;

        NController nController = mo.GetComponent<NController>();
        nController.SetGUID(monster.AssetGUID);

        Debug.Log("[client] ���� viewID = " + view.ViewID);
        //PhotonNetwork.RegisterPhotonView(view);
        photonView.RPC("SpawnComplete", RpcTarget.MasterClient);
    }

    [PunRPC]
    private void SpawnComplete()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        allocateCompleteCount++;

        if (allocateCompleteCount == PhotonNetwork.PlayerList.Length)
        {
            Debug.Log("[MasterClient] ��� �÷��̾ ���� �Ϸ�! FSM ����");
            allocateCompleteCount = 1;
            NController nController = mo.GetComponent<NController>();
            nController.StartFSM();
        }
    }
}
