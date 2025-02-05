using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    private float spawnTime = 0f;    

    void Update()
    {
        spawnTime += Time.deltaTime;
        if(spawnTime > 5f)
        {
            Debug.Log("spawn");
            StartCoroutine(ResourceManager.Instance.LoadAsset("SML2", transform.position));
            spawnTime = 0f;
        }
    }
}
