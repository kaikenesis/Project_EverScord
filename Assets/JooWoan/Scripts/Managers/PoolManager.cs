using System.Collections.Generic;
using UnityEngine;
using EverScord.Weapons;
using System;

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

        [SerializeField] private List<PoolableInfo> poolableGameObjects;
        [SerializeField] private List<BulletTracerInfo> poolableBulletTracers;
        private IDictionary<PoolableType, GameObjectPool> gameObjectPoolDict;
        private IDictionary<TracerType, BulletPool> bulletPoolDict;
        private IDictionary<Type, object> poolDict = new Dictionary<Type, object>();

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
            CreateGameObjectPool();
            CreateBulletPool();
        }

        private void CreateGameObjectPool()
        {
            if (gameObjectPoolDict != null && gameObjectPoolDict.Count > 0)
                gameObjectPoolDict.Clear();

            gameObjectPoolDict = new Dictionary<PoolableType, GameObjectPool>();

            foreach (PoolableInfo info in poolableGameObjects)
            {
                GameObjectPool pool = new GameObjectPool(info.prefab);
                pool.Root.parent = PoolRoot;
                gameObjectPoolDict[info.type] = pool;
            }
        }

        private void CreateBulletPool()
        {
            if (bulletPoolDict != null && gameObjectPoolDict.Count > 0)
                gameObjectPoolDict.Clear();
            
            bulletPoolDict = new Dictionary<TracerType, BulletPool>();

            foreach (BulletTracerInfo info in poolableBulletTracers)
            {
                BulletPool pool = new BulletPool(info.prefab);
                pool.Root.parent = PoolRoot;
                bulletPoolDict[info.type] = pool;
            }
        }
        
        private void CreatePool<T>() where T : class, new()
        {
            ObjectPool<T> pool = new();
            poolDict[typeof(T)] = pool;
        }

        public static GameObject Get(PoolableType type)
        {
            return Instance.gameObjectPoolDict[type].GetObject();
        }

        public static void Return(GameObject obj, PoolableType type)
        {
            Instance.gameObjectPoolDict[type].ReturnObject(obj);
        }

        public static Bullet Get(TracerType type)
        {
            return Instance.bulletPoolDict[type].GetObject();
        }

        public static void Return(Bullet obj, TracerType type)
        {
            Instance.bulletPoolDict[type].ReturnObject(obj);
        }

        public static T Get<T>() where T : class, new()
        {
            if (!Instance.poolDict.TryGetValue(typeof(T), out object pool))
            {
                Debug.LogWarning($"Failed to get pool type : {typeof(T)}");
                return default;
            }
            
            return ((ObjectPool<T>)pool).GetObject();
        }

        public static void Return<T>(T obj) where T : class, new()
        {
            if (!Instance.poolDict.TryGetValue(typeof(T), out object pool))
            {
                Debug.LogWarning($"Failed to get pool type : {typeof(T)}");
                return;
            }
            
            ((ObjectPool<T>)pool).ReturnObject(obj);
        }
    }

    [System.Serializable]
    public class PoolableInfo
    {
        public PoolableType type;
        public GameObject prefab;
    }

    [System.Serializable]
    public class BulletTracerInfo
    {
        public TracerType type;
        public GameObject prefab;
    }
}
