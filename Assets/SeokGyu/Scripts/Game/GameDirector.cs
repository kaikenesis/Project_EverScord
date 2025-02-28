using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

namespace EverScord
{
    public class GameDirector : MonoBehaviour
    {
        [SerializeField] private List<GameObject> photonPrefabList;
        [SerializeField] private List<PhotonCharacterInfo> photonCharacterInfoList;

        private void Awake()
        {
            GameManager.ResetPlayerInfo();

            SpawnPhotonPrefabs();
            CreatePlayer();
        }

        public void SpawnPhotonPrefabs()
        {
            DefaultPool pool = PhotonNetwork.PrefabPool as DefaultPool;
            
            if (pool == null)
                return;
            
            for (int i = 0; i < photonPrefabList.Count; i++)
            {
                if (!pool.ResourceCache.ContainsKey(photonPrefabList[i].name))
                    pool.ResourceCache.Add(photonPrefabList[i].name, photonPrefabList[i]);
                
                PhotonNetwork.Instantiate(photonPrefabList[i].name, Vector3.zero, Quaternion.identity);
            }
        }

        private void CreatePlayer()
        {
            DefaultPool pool = PhotonNetwork.PrefabPool as DefaultPool;

            foreach (PhotonCharacterInfo info in photonCharacterInfoList)
            {
                if (info.CharacterType != GameManager.Instance.PlayerData.character)
                    continue;

                if (!pool.ResourceCache.ContainsKey(info.PhotonPrefab.name))
                    pool.ResourceCache.Add(info.PhotonPrefab.name, info.PhotonPrefab);

                PhotonNetwork.Instantiate(info.PhotonPrefab.name, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
                break;
            }
        }
    }

    [System.Serializable]
    public class PhotonCharacterInfo
    {
        public PlayerData.ECharacter CharacterType;
        public GameObject PhotonPrefab;
    }
}
