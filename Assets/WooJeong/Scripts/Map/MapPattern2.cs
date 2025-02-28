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
    private float curTime = 10;
    private bool isCreate = false;
    private List<Vector3> directions = new List<Vector3>();
    private float span = 3f;
    private int randInt;
    private PhotonView photonView;
    private Coroutine spawn;
    private List<GameObject> spawnList = new();
    private Dictionary<GameObject, VisualEffect> effectDict = new();

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();

        for (int i = 0; i < 4; i++)
        {
            Vector3 direction = Quaternion.AngleAxis(180 / 4 * i, Vector3.up) * transform.forward;
            directions.Add(direction);
        }
    }

    private async void OnEnable()
    {
        if (!isCreate)
        {
            await ResourceManager.Instance.CreatePool(attackLaser.AssetGUID, 20);
            isCreate = true;
        }

        spawn = StartCoroutine(Spawn());
    }

    private void OnDisable()
    {
        StopCoroutine(spawn);
        spawn = null;
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
                randInt = Random.Range(0, 4);
                photonView.RPC("SyncMapPattern2", RpcTarget.Others, randInt);
                yield return StartCoroutine(Attack());
                curTime = 0f;
            }

            yield return new WaitForSeconds(1f);
        }
    }

    private IEnumerator Attack()
    {
        if (spawnList.Count == 0)
        {
            GameObject start = ResourceManager.Instance.GetFromPool(attackLaser.AssetGUID,
                                                        transform.position, Quaternion.identity);
            spawnList.Add(start);
            effectDict.Add(start, start.GetComponent<VisualEffect>());
            yield return new WaitForSeconds(attackTimeSpan);

            for (int i = 1; i < maxSpawn; i++)
            {
                GameObject go = ResourceManager.Instance.GetFromPool(attackLaser.AssetGUID,
                    transform.position + directions[randInt] * span * i, Quaternion.identity);
                GameObject goReverse = ResourceManager.Instance.GetFromPool(attackLaser.AssetGUID,
                    transform.position - directions[randInt] * span * i, Quaternion.identity);
                spawnList.Add(go);
                effectDict.Add(go, go.GetComponent<VisualEffect>());
                spawnList.Add(goReverse);
                effectDict.Add(goReverse, goReverse.GetComponent<VisualEffect>());
                yield return new WaitForSeconds(attackTimeSpan);
            }
        }
        else
        {
            spawnList[0].transform.position = transform.position;
            effectDict[spawnList[0]].Play();
            yield return new WaitForSeconds(attackTimeSpan);

            for (int i = 1; i < spawnList.Count; i += 2)
            {
                spawnList[i].transform.position = transform.position + directions[randInt] * span * (i + 1) / 2;
                spawnList[i + 1].transform.position = transform.position - directions[randInt] * span * (i + 1) / 2;
                effectDict[spawnList[i]].Play();
                effectDict[spawnList[i + 1]].Play();
                yield return new WaitForSeconds(attackTimeSpan);
            }
        }
    }

    [PunRPC]
    private void SyncMapPattern2(int rand)
    {
        randInt = rand;
        StartCoroutine(Attack());
    }

}
