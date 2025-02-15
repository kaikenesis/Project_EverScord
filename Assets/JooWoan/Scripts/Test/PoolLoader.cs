using UnityEngine.AddressableAssets;
using System.Collections.Generic;
using UnityEngine;

namespace EverScord.Pool
{
    public class PoolLoader : MonoBehaviour
    {
        [SerializeField] private List<AssetReferenceGameObject> assetReferenceList;

        void Awake()
        {
            LoadPools();
        }

        public async void LoadPools()
        {
            foreach (AssetReference reference in assetReferenceList)
                await ResourceManager.Instance.CreatePool(reference.AssetGUID);
        }
    }
}
