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
        [SerializeField] private List<PhotonCharacterInfo> photonCharacterInfoList;

        private void Awake()
        {
            SpawnPhotonPrefabs();
            CreatePlayer();
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

        private void CreatePlayer()
        {
            DefaultPool pool = PhotonNetwork.PrefabPool as DefaultPool;

            foreach (PhotonCharacterInfo info in photonCharacterInfoList)
            {
                if (info.CharacterType != GameManager.Instance.userData.character)
                    continue;

                if (!pool.ResourceCache.ContainsKey(info.PhotonPrefab.name))
                    pool.ResourceCache.Add(info.PhotonPrefab.name, info.PhotonPrefab);

                PhotonNetwork.Instantiate(info.PhotonPrefab.name, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
                break;
            }
        }

        public async void LoadPools()
        {
            foreach (AssetReference reference in assetReferenceList)
                await ResourceManager.Instance.CreatePool(reference.AssetGUID);
            _ = ResourceManager.Instance.CreatePool("BossProjectile", 63);
            _ = ResourceManager.Instance.CreatePool("StoneUp", 9);
            _ = ResourceManager.Instance.CreatePool("MonsterAttack");
            _ = ResourceManager.Instance.CreatePool("NML2_A1_Effect01");
            _ = ResourceManager.Instance.CreatePool("NML2_A2_Effect");
        }
    }

    [System.Serializable]
    public class PhotonCharacterInfo
    {
        public ECharacter CharacterType;
        public GameObject PhotonPrefab;
    }
}
