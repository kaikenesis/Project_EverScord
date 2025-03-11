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
            ResourceManager.ClearAllPools();
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

            _ = ResourceManager.Instance.CreatePool("DamageText");
            _ = ResourceManager.Instance.CreatePool("BossProjectile", 63);
            _ = ResourceManager.Instance.CreatePool("MonsterProjectile", 63);
            _ = ResourceManager.Instance.CreatePool("MonsterHealthBar");
            _ = ResourceManager.Instance.CreatePool("NMM2_Projectile", 5);
            _ = ResourceManager.Instance.CreatePool("StoneUp", 9);
            _ = ResourceManager.Instance.CreatePool("MonsterAttack");
            _ = ResourceManager.Instance.CreatePool("BossMonsterStoneAttack");            
            _ = ResourceManager.Instance.CreatePool("NML2_A1_Effect01");
            _ = ResourceManager.Instance.CreatePool("NML2_A2_Effect");
            _ = ResourceManager.Instance.CreatePool("P15_Attack", 1);
            _ = ResourceManager.Instance.CreatePool("StandingAttackEffect", 2);
        }
    }

    [System.Serializable]
    public class AssetReferenceInfo
    {
        public AssetReferenceGameObject Reference;
        public int PoolSize;
    }
}
