using System.Collections.Generic;
using System.Threading.Tasks;
using EverScord;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using EverScord.Pool;

public class ResourceManager : Singleton<ResourceManager>
{
    // 풀링된 오브젝트들을 담을 Dictionary
    private Dictionary<string, Queue<GameObject>> poolDictionary = new Dictionary<string, Queue<GameObject>>();

    // 스크립트가 부착된 게임 오브젝트를 풀링하고 싶을 경우 해당 클래스는 IPoolable를 상속
    private Dictionary<string, Queue<IPoolable>> iPoolableDictionary = new Dictionary<string, Queue<IPoolable>>();

    // 프리팹의 원본을 보관할 Dictionary
    private Dictionary<string, GameObject> prefabDictionary = new Dictionary<string, GameObject>();

    private Dictionary<string, Object> objectDictionary = new();
    
    public Transform PoolRoot { get; private set; }

    // 풀 크기 설정
    private const int DEFAULT_POOL_SIZE = 10;

    public T GetAsset<T>(string addressableKey) where T : class
    {
        if (!objectDictionary.ContainsKey(addressableKey))
        {
            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(addressableKey);
            handle.WaitForCompletion();
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                objectDictionary[addressableKey] = handle.Result as Object;
            }
        }

        return objectDictionary[addressableKey] as T;
    }

    // 어드레서블 에셋을 로드하고 풀 생성
    public async Task CreatePool(string addressableKey, int poolSize = DEFAULT_POOL_SIZE)
    {
        if (poolDictionary.ContainsKey(addressableKey))
        {
            Debug.LogWarning($"Pool for {addressableKey} already exists!");
            return;
        }

        // 어드레서블에서 프리팹 로드
        AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(addressableKey);
        await handle.Task;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            GameObject prefab = handle.Result;
            prefabDictionary[addressableKey] = prefab;

            if (prefab.GetComponent<IPoolable>() != null)
            {
                Queue<IPoolable> poolableObjectPool = new Queue<IPoolable>();
                iPoolableDictionary[addressableKey] = poolableObjectPool;
            }
            else
            {
                // 풀 생성
                Queue<GameObject> objectPool = new Queue<GameObject>();
                poolDictionary[addressableKey] = objectPool;
            }

            for (int i = 0; i < poolSize; i++)
            {
                GameObject obj = CreateNewObject(addressableKey);
                IPoolable poolable = obj.GetComponent<IPoolable>();

                if (poolable != null)
                    ReturnToPool(poolable, addressableKey);
                else
                    ReturnToPool(obj, addressableKey);
            }
        }
        else
        {
            Debug.LogError($"Failed to load addressable asset: {addressableKey}");
        }
    }
    
    // 풀에서 오브젝트 가져오기
    public GameObject GetFromPool(string addressableKey, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(addressableKey))
        {
            Debug.LogWarning($"Pool for {addressableKey} doesn't exist!");           
            return null;
        }

        Queue<GameObject> objectPool = poolDictionary[addressableKey];

        // 풀이 비어있으면 새로 생성
        if (objectPool.Count == 0)
        {
            GameObject newObj = CreateNewObject(addressableKey);
            objectPool.Enqueue(newObj);
        }

        GameObject obj = objectPool.Dequeue();
        obj.transform.SetPositionAndRotation(position, rotation);
        obj.SetActive(true);

        return obj;
    }

    public IPoolable GetFromPool(string addressableKey, bool returnEnabled = true)
    {
        if (!iPoolableDictionary.ContainsKey(addressableKey))
        {
            Debug.LogWarning($"IPool for {addressableKey} doesn't exist!");
            return null;
        }

        Queue<IPoolable> poolableObjectPool = iPoolableDictionary[addressableKey];

        if (poolableObjectPool.Count == 0)
        {
            IPoolable poolable1 = CreateNewObject(addressableKey).GetComponent<IPoolable>();

            if (poolable1 == null)
            {
                Debug.LogWarning($"Invalid Addressable Key : {addressableKey}");
                return null;
            }

            poolableObjectPool.Enqueue(poolable1);
        }

        IPoolable poolable2 = poolableObjectPool.Dequeue();
        poolable2.SetGameObject(returnEnabled);

        return poolable2;
    }

    // 오브젝트를 풀로 반환
    public void ReturnToPool(GameObject obj, string addressableKey)
    {
        if (!poolDictionary.ContainsKey(addressableKey))
        {
            poolDictionary[addressableKey] = new Queue<GameObject>();
        }
        obj.SetActive(false);
        poolDictionary[addressableKey].Enqueue(obj);
    }

    public void ReturnToPool(IPoolable poolable, string addressableKey)
    {
        if (!iPoolableDictionary.ContainsKey(addressableKey))
        {
            iPoolableDictionary[addressableKey] = new Queue<IPoolable>();
        }

        poolable.SetGameObject(false);
        iPoolableDictionary[addressableKey].Enqueue(poolable);
    }

    private GameObject CreateNewObject(string addressableKey)
    {
        if (!prefabDictionary.ContainsKey(addressableKey))
        {
            Debug.LogError($"Prefab for {addressableKey} not found!");
            return null;
        }

        SetPoolRoot();

        GameObject obj = Instantiate(prefabDictionary[addressableKey]);
        obj.name = $"{prefabDictionary[addressableKey].name}_Pooled";
        obj.transform.SetParent(PoolRoot);
        return obj;
    }

    public void Destroy(string addressableKey)
    {
        foreach (var prefab in poolDictionary[addressableKey])
        {
            Destroy(prefab);
        }
        poolDictionary.Remove(addressableKey);
        prefabDictionary.Remove(addressableKey);

        Addressables.Release(addressableKey);
    }

    private void SetPoolRoot()
    {
        if (PoolRoot)
            return;
        
        PoolRoot = GameObject.FindGameObjectWithTag(ConstStrings.TAG_POOLROOT).transform;

        if (!PoolRoot)
            PoolRoot = new GameObject("PoolRoot").transform;
    }

    //private void OnDestroy()
    //{
    //    // 어드레서블 에셋 해제
    //    foreach (var prefab in prefabDictionary.Values)
    //    {
    //        Addressables.Release(prefab);
    //    }
    //}
}
