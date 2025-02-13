using Photon.Pun;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine;

namespace EverScord
{
    public class GameDirector : MonoBehaviour
    {
        [SerializeField] private List<AssetReferenceGameObject> assetReferenceList;
        [SerializeField] private List<GameObject> photonPrefabList;
        [SerializeField] private GameObject playerObj;

        private void Awake()
        {
            // CreatePlayer();
            SpawnPhotonPrefabs();
            LoadPools();
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

        public async void LoadPools()
        {
            foreach (AssetReference reference in assetReferenceList)
                await ResourceManager.Instance.CreatePool(reference.AssetGUID);
        }
    }
}
