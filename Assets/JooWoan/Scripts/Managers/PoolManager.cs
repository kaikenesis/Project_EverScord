using System.Collections.Generic;
using UnityEngine;

namespace EverScord.Pool
{
    public class PoolManager : MonoBehaviour
    {
        #region Singleton
        public static PoolManager Instance { get; private set; }
        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                Init();
            }
            else
                Destroy(gameObject);
        }
        #endregion

        [SerializeField] private List<PoolableInfo> poolableInfos;
        private IDictionary<string, ObjectPool> poolDict = new Dictionary<string, ObjectPool>();
        private Transform poolRoot = null;
        public Transform PoolRoot
        {
            get
            {
                if (poolRoot == null)
                    poolRoot = GameObject.FindGameObjectWithTag(ConstStrings.TAG_POOLROOT).transform;

                return poolRoot;
            }
        }

        public void Init()
        {
            if (poolDict.Count > 0)
                poolDict.Clear();

            poolDict = new Dictionary<string, ObjectPool>();

            foreach (PoolableInfo info in poolableInfos)
                CreatePool(info);
        }

        private void CreatePool(PoolableInfo info)
        {
            ObjectPool pool = new ObjectPool(info.poolablePrefab, info.initCount);
            pool.Root.parent = PoolRoot;
            poolDict[pool.OriginalPrefab.name] = pool;
        }

        public static GameObject GetObject(string poolableType)
        {
            if (!Instance.poolDict.ContainsKey(poolableType))
            {
                Debug.LogWarning($"Poolable type : '{poolableType}' does not exist.");
                return null;
            }

            return Instance.poolDict[poolableType].GetObject();
        }

        public static void ReturnObject(GameObject obj)
        {
            if (!Instance.poolDict.ContainsKey(obj.name))
            {
                Destroy(obj);
                return;
            }

            Instance.poolDict[obj.name].ReturnObject(obj);
        }
    }

    [System.Serializable]
    public class PoolableInfo
    {
        public GameObject poolablePrefab;
        public int initCount;
    }
}
