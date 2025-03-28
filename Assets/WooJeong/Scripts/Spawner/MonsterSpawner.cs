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
    [SerializeField] private float startDelay = 0f;
    private float curTime = 0;
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
    }

    private void OnDestroy()
    {
        LevelControl.OnProgressUpdated -= ProgressCheck;
    }

    public static void ActivateSpawners()
    {
        var spawners = FindObjectsOfType<MonsterSpawner>();

        foreach (MonsterSpawner spawner in spawners)
            spawner.StartSpawn();
    }

    public void StartSpawn()
    {
        spawn = StartCoroutine(Spawn());
    }

    protected void ProgressCheck(float currentProgress)
    {
        if (currentProgress >= 1f)
        {
            photonView.RPC(nameof(SyncSpawnMarker), RpcTarget.All, false);
            gameObject.SetActive(false);
        }
    }

    private IEnumerator Spawn()
    {
        if(!PhotonNetwork.IsMasterClient)
            yield break;

        float timeUntilSpawn = 0f;

        // yield return new WaitForSeconds(startDelay);
        for (float t = 0f; t <= startDelay; t += Time.deltaTime)
        {
            timeUntilSpawn = startDelay - t;

            if (enableMarker && timeUntilSpawn < SPAWN_ALERT_TIME)
                photonView.RPC(nameof(SyncSpawnMarker), RpcTarget.All, true);

            yield return null;
        }

        curTime = spawnTimer;
        while(true)
        {
            curTime++;
            timeUntilSpawn = spawnTimer - curTime;

            if (AllPlayerDead() == true)
                yield break;

            if (curTime > spawnTimer)
            {
                mo = ResourceManager.Instance.GetFromPool(monster.AssetGUID, transform.position, Quaternion.identity);

                if (mo == null)
                {
                    curTime = 0f;
                    yield return new WaitForSeconds(1f);
                    continue;
                }

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
                    allocateCompleteCount = 1;
                    nController.StartFSM();
                }

                curTime = 0f;

                photonView.RPC(nameof(SyncSpawnMarker), RpcTarget.All, false);
            }
            else if (enableMarker && timeUntilSpawn < SPAWN_ALERT_TIME)
                photonView.RPC(nameof(SyncSpawnMarker), RpcTarget.All, true);

            yield return new WaitForSeconds(1f);
        }
    }

    private bool AllPlayerDead()
    {
        int playerCount = 0;
        foreach (var player in GameManager.Instance.PlayerDict.Values)
        {
            if (player.IsDead)
                playerCount++;
        }
        if (playerCount != GameManager.Instance.PlayerDict.Count)
            return false;

        return true;
    }

    private void SpawnSmoke()
    {
        spawnSmoke = ResourceManager.Instance.GetFromPool(AssetReferenceManager.SpawnSmoke_ID) as PooledParticle;
        
        if (spawnSmoke == null)
            return;
        
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
            if (spawnMarker)
                spawnMarker.SetActive(true);
        }
        else if (spawnMarker != null)
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
            allocateCompleteCount = 1;
            NController nController = mo.GetComponent<NController>();
            nController.StartFSM();
        }
    }
}
