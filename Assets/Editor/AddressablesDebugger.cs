using UnityEditor;
using UnityEngine;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;

public class AddressablesDebugger
{
    [MenuItem("Tools/Debug Addressables Paths")]
    public static void DebugAddressablesPaths()
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        var profileSettings = settings.profileSettings;
        string activeProfileId = settings.activeProfileId;

        // 프로필 변수 출력
        Debug.Log("===== 프로필 변수 =====");
        foreach (var variableName in profileSettings.GetVariableNames())
        {
            var value = profileSettings.GetValueByName(activeProfileId, variableName);
            var resolvedValue = profileSettings.EvaluateString(activeProfileId, value);
            Debug.Log($"{variableName} = '{value}' -> '{resolvedValue}'");
        }

        // 그룹 설정 출력
        Debug.Log("===== 그룹 설정 =====");
        foreach (var group in settings.groups)
        {
            Debug.Log($"그룹: {group.Name}");
            var schema = group.GetSchema<UnityEditor.AddressableAssets.Settings.GroupSchemas.BundledAssetGroupSchema>();
            if (schema != null)
            {
                var buildPath = schema.BuildPath.GetValue(settings);
                var loadPath = schema.LoadPath.GetValue(settings);
                Debug.Log($"  Build Path: {buildPath}");
                Debug.Log($"  Load Path: {loadPath}");
                Debug.Log($"  Bundle Mode: {schema.BundleMode}");
                Debug.Log($"  Is Remote: {!schema.IncludeInBuild}");
            }
        }
    }
}