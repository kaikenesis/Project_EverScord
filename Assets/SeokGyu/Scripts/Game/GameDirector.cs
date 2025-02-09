using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

namespace EverScord
{
    public class GameDirector : MonoBehaviour
    {
        [SerializeField] private List<GameObject> photonPrefabList;
        [SerializeField] private GameObject playerObj;

        private void Awake()
        {
            // CreatePlayer();
            SpawnPhotonPrefabs();
        }

        private void CreatePlayer()
        {
            //Transform[] points = pointGroup.GetComponentsInChildren<Transform>();
            //int idx = Random.Range(1, points.Length);
            DefaultPool pool = PhotonNetwork.PrefabPool as DefaultPool;
            pool.ResourceCache.Add(playerObj.name, playerObj);
            PhotonNetwork.Instantiate(playerObj.name, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
        }

        public void SpawnPhotonPrefabs()
        {
            DefaultPool pool = PhotonNetwork.PrefabPool as DefaultPool;
            
            if (pool == null)
                return;
            
            for (int i = 0; i < photonPrefabList.Count; i++)
            {
                if (pool.ResourceCache.ContainsKey(photonPrefabList[i].name))
                    continue;

                pool.ResourceCache.Add(photonPrefabList[i].name, photonPrefabList[i]);
                PhotonNetwork.Instantiate(photonPrefabList[i].name, Vector3.zero, Quaternion.identity);
            }
        }
    }
}
