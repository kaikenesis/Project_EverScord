using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public GameObject monsterPrefab;
    DefaultPool pool;

    private void Start()
    {
        pool = PhotonNetwork.PrefabPool as DefaultPool;
        pool.ResourceCache.Add("monsterPrefab", monsterPrefab);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            PhotonNetwork.Instantiate("monsterPrefab", transform.position, new Quaternion(0, 0, 0, 0));
        }
    }
}
