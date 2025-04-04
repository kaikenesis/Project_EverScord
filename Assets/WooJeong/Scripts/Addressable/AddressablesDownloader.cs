using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AddressablesDownloader : MonoBehaviour
{
    [SerializeField] private string labelToPreload = "";
    [SerializeField] private Slider slider;

    private AsyncOperationHandle _downloadHandle;

    private void Start()
    {
        StartCoroutine(InitializeGame());
    }

    private IEnumerator InitializeGame()
    {
        var checkCatalogOperation = Addressables.CheckForCatalogUpdates();
        yield return checkCatalogOperation;

        if (checkCatalogOperation.Result.Count > 0)
        {
            // 카탈로그 업데이트가 필요한 경우
            var updateOperation = Addressables.UpdateCatalogs(checkCatalogOperation.Result);
            yield return updateOperation;

            // 새 콘텐츠 다운로드
            yield return StartCoroutine(PreloadRemoteAssets());
        }
        else
        {
            Debug.Log("모든 콘텐츠가 최신 상태입니다. 다운로드 필요 없음.");
        }
        
        StartGame();
    }

    private void StartGame()
    {
        SceneManager.LoadScene("LobbyScene");
    }

    public IEnumerator PreloadRemoteAssets()
    {
        Debug.Log("시작: 원격 에셋 다운로드");

        _downloadHandle = Addressables.DownloadDependenciesAsync(labelToPreload, false);

        while (!_downloadHandle.IsDone)
        {
            float progress = _downloadHandle.PercentComplete;
            if (slider != null)
                slider.value = progress; // SetProgress(progress);

            Debug.Log($"다운로드 진행률: {progress * 100}%");
            yield return null;
        }

        if (_downloadHandle.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log("원격 에셋 다운로드 완료!");
            // 실제 메모리 로드는 LoadAssetAsync 등의 메서드를 사용할 때 발생
        }
        else
        {
            Debug.LogError($"다운로드 실패: {_downloadHandle.OperationException}");
        }
    }

    private void OnDestroy()
    {
        if (_downloadHandle.IsValid())
            Addressables.Release(_downloadHandle);
    }
}