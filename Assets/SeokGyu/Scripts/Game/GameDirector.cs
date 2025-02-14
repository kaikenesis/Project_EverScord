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

        private void Awake()
        {
            SpawnPhotonPrefabs();
            LoadPools();
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
