using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField] private AssetReferenceGameObject monster;
    [SerializeField] private float spawnTime = 0f;
    private int spawnCount = 0;

    void Update()
    {
        if(!PhotonNetwork.IsMasterClient)
            return;

        spawnTime += Time.deltaTime;
        if(spawnTime > 5f && spawnCount <= 0)
        {
            StartCoroutine(ResourceManager.Instance.SpawnMonster(monster.AssetGUID, transform.position));
            spawnCount++;
            spawnTime = 0f;
        }
    }
}
