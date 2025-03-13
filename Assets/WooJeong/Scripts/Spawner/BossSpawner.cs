using Photon.Pun;
using System.Collections;
using UnityEngine;
using EverScord.Effects;
using EverScord;

public class BossSpawner : MonoBehaviour
{
    [SerializeField] private float spawnTimer = 0f;

    private float curTime = 0;
    private int spawnCount = 0;

    private object data;
    private GameObject mo;
    private PhotonView photonView;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        GameObject bossPrefab = ResourceManager.Instance.GetAsset<GameObject>(AssetReferenceManager.Boss_ID);
        mo = Instantiate(bossPrefab, transform.position, Quaternion.identity);
        
        mo.SetActive(false);
    }

    void Start()
    {
        LevelControl.OnProgressUpdated += ProgressCheck;
    }

    private void OnDestroy()
    {
        LevelControl.OnProgressUpdated -= ProgressCheck;
    }

    private void ProgressCheck(float currentProgress)
    {
        if(currentProgress == 1)
        {
            StartCoroutine(Spawn());
        }
    }

    private IEnumerator Spawn()
    {
        if (!PhotonNetwork.IsMasterClient)
            yield break;
        Debug.Log("[MasterCient] 몬스터 스폰 함수 실행");
        while (true)
        {
            curTime += Time.deltaTime;
            if (curTime > spawnTimer && spawnCount <= 0)
            {
                Debug.Log("스폰");

                mo.SetActive(true);

                PhotonView view = mo.GetComponent<PhotonView>();
                if(view.ViewID == 0)
                {
                    if (PhotonNetwork.AllocateViewID(view))
                    {
                        data = view.ViewID;
                        photonView.RPC("SyncSpawn", RpcTarget.Others, data);
                    }
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

        mo.SetActive(true);

        PhotonView view = mo.GetComponent<PhotonView>();
        view.ViewID = viewID;
        Debug.Log("[client] 몬스터 viewID = " + view.ViewID);
    }
}
