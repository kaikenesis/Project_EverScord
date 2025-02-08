using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ResourceManager : Singleton<ResourceManager>//, IOnEventCallback
{
    // 풀링된 오브젝트들을 담을 Dictionary
    private Dictionary<string, Queue<GameObject>> poolDictionary = new Dictionary<string, Queue<GameObject>>();

    // 프리팹의 원본을 보관할 Dictionary
    private Dictionary<string, GameObject> prefabDictionary = new Dictionary<string, GameObject>();

    // 풀 크기 설정
    private const int DEFAULT_POOL_SIZE = 10;

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

            // 풀 생성
            Queue<GameObject> objectPool = new Queue<GameObject>();
            for (int i = 0; i < poolSize; i++)
            {
                GameObject obj = CreateNewObject(addressableKey);
                ReturnToPool(obj, addressableKey);
            }

            poolDictionary[addressableKey] = objectPool;
        }
        else
        {
            Debug.LogError($"Failed to load addressable asset: {addressableKey}");
        }
    }

    // 풀에서 오브젝트 가져오기
    public GameObject GetFromPool(string addressableKey, Vector3 position, Quaternion rotation)
    {
        Debug.Log("GetFromPool");
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

    private GameObject CreateNewObject(string addressableKey)
    {
        if (!prefabDictionary.ContainsKey(addressableKey))
        {
            Debug.LogError($"Prefab for {addressableKey} not found!");
            return null;
        }

        GameObject obj = Instantiate(prefabDictionary[addressableKey]);
        obj.name = $"{addressableKey}_pooled";
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

    //private void OnDestroy()
    //{
    //    // 어드레서블 에셋 해제
    //    foreach (var prefab in prefabDictionary.Values)
    //    {
    //        Addressables.Release(prefab);
    //    }
    //}
}
