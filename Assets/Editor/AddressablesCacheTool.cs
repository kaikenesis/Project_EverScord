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
        // 에디터 캐시 삭제
        bool cleared = Caching.ClearCache();

        // 어드레서블 플레이어 데이터 정리
        AddressableAssetSettings.CleanPlayerContent();

        // 캐시 폴더 삭제 시도
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
                Debug.LogError($"캐시 폴더 삭제 실패: {e.Message}");
            }
        }

        // 결과 표시
        if (cleared || folderDeleted)
        {
            Debug.Log("어드레서블 캐시가 성공적으로 삭제되었습니다.");
            EditorUtility.DisplayDialog("캐시 삭제 완료", "어드레서블 캐시가 성공적으로 삭제되었습니다.", "확인");
        }
        else
        {
            Debug.LogWarning("어드레서블 캐시를 찾을 수 없거나 삭제할 수 없습니다.");
            EditorUtility.DisplayDialog("알림", "삭제할 어드레서블 캐시를 찾을 수 없습니다.", "확인");
        }
    }
}