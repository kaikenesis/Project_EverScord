using System.Collections.Generic;
using UnityEngine;
using EverScord.Weapons;

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

        [SerializeField] private List<PoolableGameObjectInfo> poolableGameObjects;
        private IDictionary<GameObjectPool.PoolType, GameObjectPool> gameObjectPoolDict = new Dictionary<GameObjectPool.PoolType, GameObjectPool>();
        private ObjectPool<Bullet> bulletPool = new ObjectPool<Bullet>();

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
        }

        private void CreateGameObjectPool()
        {
            if (gameObjectPoolDict.Count > 0)
                gameObjectPoolDict.Clear();

            gameObjectPoolDict = new Dictionary<GameObjectPool.PoolType, GameObjectPool>();

            foreach (PoolableGameObjectInfo info in poolableGameObjects)
            {
                GameObjectPool pool = new GameObjectPool(info.prefab);
                pool.Root.parent = PoolRoot;
                gameObjectPoolDict[info.type] = pool;
            }
        }

        public static GameObject Get(GameObjectPool.PoolType type)
        {
            return Instance.gameObjectPoolDict[type].GetObject();
        }

        public static void Return(GameObject obj, GameObjectPool.PoolType type)
        {
            Instance.gameObjectPoolDict[type].ReturnObject(obj);
        }

    }

    [System.Serializable]
    public class PoolableGameObjectInfo
    {
        public GameObjectPool.PoolType type;
        public GameObject prefab;
    }
}
