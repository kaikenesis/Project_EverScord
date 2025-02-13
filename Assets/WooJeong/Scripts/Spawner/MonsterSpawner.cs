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
    private int allocateCompleteCount = 1;

    private const byte spawnEvent = 1;
    private const byte allocateViewIDEvent = 2;
    private const byte allocateCompleteEventCode = 3;
    private object data;
    private RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
    private SendOptions sendOptions = new SendOptions { Reliability = true };
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
        Debug.Log("[MasterCient] 몬스터 스폰 함수 실행");
        while(true)
        {
            curTime += Time.deltaTime;
            if (curTime > spawnTimer && spawnCount <= 0)
            {
                Debug.Log("스폰");
                mo = ResourceManager.Instance.GetFromPool(monster.AssetGUID, transform.position, Quaternion.identity);

                PhotonView view = mo.GetComponent<PhotonView>();

                if (PhotonNetwork.AllocateViewID(view))
                {
                    data = view.ViewID;
                    photonView.RPC("SyncSpawn", RpcTarget.Others, data);
                }

                if(spawnCount ==0)
                {
                    NController nController = mo.GetComponent<NController>();
                    nController.SetGUID(monster.AssetGUID);
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
        if (spawnCount == 0)
        {
            NController nController = mo.GetComponent<NController>();
            nController.SetGUID(monster.AssetGUID);
        }
        spawnCount++;
        Debug.Log("[client] 몬스터 viewID = " + view.ViewID);
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
            Debug.Log("[MasterClient] 모든 플레이어가 스폰 완료! FSM 시작");
            allocateCompleteCount = 1;
            NController nController = mo.GetComponent<NController>();
            nController.StartFSM();
        }
    }
}
