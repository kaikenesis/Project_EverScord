using EverScord;
using EverScord.Effects;
using EverScord.Pool;
using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class MonsterSpawner : MonoBehaviour
{
    private const float SPAWN_ALERT_TIME = 3f;

    [SerializeField] private AssetReferenceGameObject monster;
    [SerializeField] private float spawnTimer = 0f;
    private float curTime = 0;
    private int spawnCount = 0;
    private int allocateCompleteCount = 1;
    private bool enableMarker = true;

    private int data;
    private GameObject mo, spawnMarker;
    private PooledParticle spawnSmoke;
    private PhotonView photonView;
    private Coroutine spawn;

    void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    private async void Start()
    {
        await ResourceManager.Instance.CreatePool(monster.AssetGUID, 1);
        LevelControl.OnProgressUpdated += ProgressCheck;
        spawn = StartCoroutine(Spawn());
    }

    protected void ProgressCheck(float currentProgress)
    {
        if (currentProgress == 1)
        {
            gameObject.SetActive(false);
        }
    }

    private IEnumerator Spawn()
    {
        if(!PhotonNetwork.IsMasterClient)
            yield break;
        Debug.Log("[MasterCient] 몬스터 스폰 함수 실행");
        while(true)
        {
            curTime += Time.deltaTime;
            float timeUntilSpawn = spawnTimer - curTime;

            if (curTime > spawnTimer)
            {
                Debug.Log("스폰");
                mo = ResourceManager.Instance.GetFromPool(monster.AssetGUID, transform.position, Quaternion.identity);

                SpawnSmoke();

                PhotonView view = mo.GetComponent<PhotonView>();
                if (view.ViewID == 0)
                {
                    if (PhotonNetwork.AllocateViewID(view))
                    {
                        data = view.ViewID;
                        photonView.RPC("SyncSpawn", RpcTarget.Others, data);
                    }
                }
                else
                {
                    photonView.RPC("SyncSpawn", RpcTarget.Others, view.ViewID);
                }

                NController nController = mo.GetComponent<NController>();
                nController.SetGUID(monster.AssetGUID);

                if (allocateCompleteCount == PhotonNetwork.PlayerList.Length)
                {
                    Debug.Log("[MasterClient] 모든 플레이어가 스폰 완료! FSM 시작");
                    allocateCompleteCount = 1;
                    nController.StartFSM();
                }

                spawnCount++;
                curTime = 0f;

                photonView.RPC(nameof(SyncSpawnMarker), RpcTarget.All, false);
            }
            else if (enableMarker && timeUntilSpawn < SPAWN_ALERT_TIME)
                photonView.RPC(nameof(SyncSpawnMarker), RpcTarget.All, true);

            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    private void SpawnSmoke()
    {
        spawnSmoke = ResourceManager.Instance.GetFromPool(AssetReferenceManager.SpawnSmoke_ID) as PooledParticle;
        spawnSmoke.Init(AssetReferenceManager.SpawnSmoke_ID);

        spawnSmoke.transform.position = transform.position;
        spawnSmoke.Emit();
    }

    [PunRPC]
    private void SyncSpawnMarker(bool state)
    {
        enableMarker = !state;

        if (state)
        {
            spawnMarker = ResourceManager.Instance.GetFromPool(AssetReferenceManager.SpawnMarker_ID, transform.position, Quaternion.identity);
            spawnMarker.SetActive(true);
        }
        else
            ResourceManager.Instance.ReturnToPool(spawnMarker, AssetReferenceManager.SpawnMarker_ID);
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

        SpawnSmoke();

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
