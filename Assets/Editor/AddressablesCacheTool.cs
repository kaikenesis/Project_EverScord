using System.IO;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

public static class AddressablesCacheUtility
{
    [MenuItem("Tools/Addressables/Clear Cache")]
    public static void ClearCache()
    {
        // ������ ĳ�� ����
        bool cleared = Caching.ClearCache();

        // ��巹���� �÷��̾� ������ ����
        AddressableAssetSettings.CleanPlayerContent();

        // ĳ�� ���� ���� �õ�
        string cachePath = Application.persistentDataPath + "/com.unity.addressables";
        bool folderDeleted = false;

        if (Directory.Exists(cachePath))
        {
            try
            {
                Directory.Delete(cachePath, true);
                folderDeleted = true;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"ĳ�� ���� ���� ����: {e.Message}");
            }
        }

        // ��� ǥ��
        if (cleared || folderDeleted)
        {
            Debug.Log("��巹���� ĳ�ð� ���������� �����Ǿ����ϴ�.");
            EditorUtility.DisplayDialog("ĳ�� ���� �Ϸ�", "��巹���� ĳ�ð� ���������� �����Ǿ����ϴ�.", "Ȯ��");
        }
        else
        {
            Debug.LogWarning("��巹���� ĳ�ø� ã�� �� ���ų� ������ �� �����ϴ�.");
            EditorUtility.DisplayDialog("�˸�", "������ ��巹���� ĳ�ø� ã�� �� �����ϴ�.", "Ȯ��");
        }
    }
}