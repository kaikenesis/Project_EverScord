using UnityEngine.AddressableAssets;
using System.Collections.Generic;
using UnityEngine;

namespace EverScord.Pool
{
    public class PoolLoader : MonoBehaviour
    {
        [SerializeField] private List<AssetReferenceInfo> assetReferenceList;

        void Awake()
        {
            LoadPools();
        }

        public async void LoadPools()
        {
            foreach (var info in assetReferenceList)
            {
                if (info.PoolSize == 0)
                    await ResourceManager.Instance.CreatePool(info.Reference.AssetGUID);
                else
                    await ResourceManager.Instance.CreatePool(info.Reference.AssetGUID, info.PoolSize);
            }
        }
    }

    [System.Serializable]
    public class AssetReferenceInfo
    {
        public AssetReferenceGameObject Reference;
        public int PoolSize;
    }
}
