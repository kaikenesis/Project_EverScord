using System.IO;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using UnityEngine.AddressableAssets;

public static class AddressablesCacheUtility
{
    [MenuItem("Tools/Addressables/Clear Cache")]
    public static void ClearCache()
    {
        Addressables.ClearDependencyCacheAsync("remote");
    }
}