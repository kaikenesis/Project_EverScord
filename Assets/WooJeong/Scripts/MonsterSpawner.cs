using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    private float spawnTime = 0f;
    private int spawnCount = 0;

    private void Awake()
    {
        var test = ResourceManager.Instance.test;
    }

    void Update()
    {
        if(!PhotonNetwork.IsMasterClient)
            return;

        spawnTime += Time.deltaTime;
        if(spawnTime > 5f && spawnCount <= 0)
        {
            StartCoroutine(ResourceManager.Instance.LoadAsset("SML2", transform.position));
            spawnCount++;
            spawnTime = 0f;
        }
    }
}
