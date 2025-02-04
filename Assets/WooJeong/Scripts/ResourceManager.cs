using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ResourceManager : MonoBehaviour
{
    private AsyncOperationHandle<GameObject> loadOpHandle;
    private GameObject assetInstance;

    private void LoadAsset(string address)
    {
        loadOpHandle = Addressables.LoadAssetAsync<GameObject>(address);
        loadOpHandle.Completed += OnHatLoadComplete;
    }

    private void OnHatLoadComplete(AsyncOperationHandle<GameObject> asyncOperationHandle)
    {
        if (asyncOperationHandle.Status == AsyncOperationStatus.Succeeded)
        {
            assetInstance = Instantiate(asyncOperationHandle.Result);
        }
    }
}
