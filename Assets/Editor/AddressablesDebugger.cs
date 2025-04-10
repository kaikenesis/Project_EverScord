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

        // ������ ���� ���
        Debug.Log("===== ������ ���� =====");
        foreach (var variableName in profileSettings.GetVariableNames())
        {
            var value = profileSettings.GetValueByName(activeProfileId, variableName);
            var resolvedValue = profileSettings.EvaluateString(activeProfileId, value);
            Debug.Log($"{variableName} = '{value}' -> '{resolvedValue}'");
        }

        // �׷� ���� ���
        Debug.Log("===== �׷� ���� =====");
        foreach (var group in settings.groups)
        {
            Debug.Log($"�׷�: {group.Name}");
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