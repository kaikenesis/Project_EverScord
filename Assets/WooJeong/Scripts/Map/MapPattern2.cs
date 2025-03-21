using EverScord;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.VFX;

public class MapPattern2 : MonoBehaviour
{
    [SerializeField] private AssetReferenceGameObject attackLaser;
    [SerializeField] private int maxSpawn = 10;
    [SerializeField] private float patternTimer = 10f;
    [SerializeField] private float attackTimeSpan = 0.3f;
    private float curTime = 0;
    private List<Vector3> directions = new List<Vector3>();
    private float span = 3f;
    private PhotonView photonView;
    private List<GameObject> spawnList = new();    
    private Dictionary<GameObject, VisualEffect> effectDict = new();
    private Dictionary<GameObject, MapPattern2_Attack> attackDict = new();

    private void Awake()
    {
        Debug.Log("map2 awake");
        photonView = GetComponent<PhotonView>();

        for (int i = 0; i < 4; i++)
        {
            Vector3 direction = Quaternion.AngleAxis(180 / 4 * i, Vector3.up) * transform.forward;
            directions.Add(direction);
        }
    }

    private async void Start()
    {
        if (!ResourceManager.Instance.IsPoolExist(attackLaser.AssetGUID))
            await ResourceManager.Instance.CreatePool(attackLaser.AssetGUID, 20);
        LevelControl.OnProgressUpdated += ProgressCheck;
        StartCoroutine(Spawn());
    }

    private void OnDestroy()
    {
        LevelControl.OnProgressUpdated -= ProgressCheck;
    }

    private IEnumerator Spawn()
    {
        if (!PhotonNetwork.IsMasterClient)
            yield break;

        while (true)
        {
            curTime++;
            if (curTime > patternTimer)
            {
                int randInt = Random.Range(0, 4);
                photonView.RPC("SyncMapPattern2", RpcTarget.Others, randInt);
                yield return StartCoroutine(Attack(randInt));
                curTime = 0f;
            }

            yield return new WaitForSeconds(1f);
        }
    }

    private IEnumerator Attack(int randInt)
    {
        if (spawnList.Count == 0)
        {
            GameObject start = ResourceManager.Instance.GetFromPool(attackLaser.AssetGUID,
                                                        transform.position, Quaternion.identity);
            spawnList.Add(start);
            effectDict.Add(start, start.GetComponent<VisualEffect>());
            attackDict.Add(start, start.GetComponent<MapPattern2_Attack>());
            attackDict[start].Attack();
            SoundManager.Instance.PlaySound("SoundMap2_Attack");

            yield return new WaitForSeconds(attackTimeSpan);

            for (int i = 1; i < maxSpawn; i++)
            {
                GameObject go = ResourceManager.Instance.GetFromPool(attackLaser.AssetGUID,
                    transform.position + directions[randInt] * span * i, Quaternion.identity);
                GameObject goReverse = ResourceManager.Instance.GetFromPool(attackLaser.AssetGUID,
                    transform.position - directions[randInt] * span * i, Quaternion.identity);
                spawnList.Add(go);
                effectDict.Add(go, go.GetComponent<VisualEffect>());
                attackDict.Add(go, go.GetComponent<MapPattern2_Attack>());
                spawnList.Add(goReverse);
                effectDict.Add(goReverse, goReverse.GetComponent<VisualEffect>());
                attackDict.Add(goReverse, goReverse.GetComponent<MapPattern2_Attack>());
                SoundManager.Instance.PlaySound("SoundMap2_Attack");

                attackDict[go].Attack();
                attackDict[goReverse].Attack();
                yield return new WaitForSeconds(attackTimeSpan);
            }
        }
        else
        {
            SoundManager.Instance.PlaySound("SoundMap2_Attack");
            spawnList[0].transform.position = transform.position;
            effectDict[spawnList[0]].Play();
            attackDict[spawnList[0]].Attack();
            yield return new WaitForSeconds(attackTimeSpan);

            for (int i = 1; i < spawnList.Count; i += 2)
            {
                spawnList[i].transform.position = transform.position + directions[randInt] * span * (i + 1) / 2;
                spawnList[i + 1].transform.position = transform.position - directions[randInt] * span * (i + 1) / 2;
                effectDict[spawnList[i]].Play();
                effectDict[spawnList[i + 1]].Play();
                SoundManager.Instance.PlaySound("SoundMap2_Attack");

                attackDict[spawnList[i]].Attack();
                attackDict[spawnList[i+1]].Attack();
                yield return new WaitForSeconds(attackTimeSpan);
            }
        }
    }

    [PunRPC]
    private void SyncMapPattern2(int rand)
    {
        StartCoroutine(Attack(rand));
    }

    protected void ProgressCheck(float currentProgress)
    {
        if (currentProgress == 1 && GameManager.Instance.LevelController.MaxLevelIndex != GameManager.CurrentLevelIndex)
        {
            // 현재 진행도 체크하고 다 됐으면 죽임
            gameObject.SetActive(false);
        }
    }
}
