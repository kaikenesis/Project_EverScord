using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AddressablesDownloader : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI sizeInfoText; // 다운로드 크기 정보를 표시할 Text
    [SerializeField] private TextMeshProUGUI progressText; // 다운로드 진행률을 표시할 Text
    [SerializeField] private Button downloadButton; // 다운로드 시작 버튼

    private AsyncOperationHandle _downloadHandle;
    private List<object> _keysToDownload = new List<object>();
    private long _totalDownloadSize = 0;
    private long _downloadedBytes = 0;

    private void Start()
    {
        downloadButton.onClick.AddListener(DownloadStart);
        StartCoroutine(CheckUpdate());
    }

    private IEnumerator CheckUpdate()
    {
        // UI 초기화
        if (slider != null) slider.value = 0;
        if (sizeInfoText != null) sizeInfoText.text = "업데이트 확인 중...";
        if (progressText != null) progressText.text = "";

        var checkCatalogOperation = Addressables.CheckForCatalogUpdates();
        yield return checkCatalogOperation;
        
        if (checkCatalogOperation.Result.Count > 0)
        {
            // 카탈로그 업데이트가 필요한 경우
            if (sizeInfoText != null) sizeInfoText.text = "카탈로그 업데이트 중...";

            var updateOperation = Addressables.UpdateCatalogs(checkCatalogOperation.Result);
            yield return updateOperation;

            // 다운로드가 필요한 키와 총 크기 계산
            _keysToDownload.Clear();
            _totalDownloadSize = 0;

            if (sizeInfoText != null) sizeInfoText.text = "다운로드 크기 계산 중...";

            foreach (var locator in updateOperation.Result)
            {
                foreach (var key in locator.Keys)
                {
                    var downloadSize = Addressables.GetDownloadSizeAsync(key);
                    yield return downloadSize;

                    if (downloadSize.Result > 0)
                    {
                        _keysToDownload.Add(key);
                        _totalDownloadSize += downloadSize.Result;
                        Debug.Log($"다운로드 필요한 키: {key}, 크기: {downloadSize.Result} 바이트");
                    }

                    Addressables.Release(downloadSize);
                }
            }

            // 총 다운로드 크기 표시
            string sizeText = FormatFileSize(_totalDownloadSize);
            if (sizeInfoText != null) sizeInfoText.text = $"다운로드 필요: {sizeText}";

            // 사용자에게 다운로드 시작 알림 (필요한 경우 여기에 확인 다이얼로그 추가 가능)
            Debug.Log($"총 {_keysToDownload.Count}개 에셋, {sizeText} 다운로드 필요");

            downloadButton.enabled = true;

            //// 각 키 다운로드
            //_downloadedBytes = 0;
            //foreach (var key in _keysToDownload)
            //{
            //    yield return StartCoroutine(DownloadRemoteAssets(key));
            //}

            //if (sizeInfoText != null) sizeInfoText.text = "다운로드 완료!";
            //if (progressText != null) progressText.text = "100%";

            //Addressables.Release(updateOperation);
        }
        else
        {
            Debug.Log("모든 콘텐츠가 최신 상태입니다. 다운로드 필요 없음.");
            if (sizeInfoText != null) sizeInfoText.text = "모든 콘텐츠가 최신 상태입니다.";
            StartGame();
        }

        Addressables.Release(checkCatalogOperation);
    }

    private void DownloadStart()
    {
        StartCoroutine(CoDownloadStart());
    }

    private IEnumerator CoDownloadStart()
    {
        slider.enabled = true;

        _downloadedBytes = 0;
        foreach (var key in _keysToDownload)
        {
            yield return StartCoroutine(DownloadRemoteAssets(key));
        }

        if (sizeInfoText != null) sizeInfoText.text = "다운로드 완료!";
        if (progressText != null) progressText.text = "100%";
        
        // 다운로드 완료 후 잠시 대기 (사용자가 완료 메시지를 볼 수 있도록)
        yield return new WaitForSeconds(1.5f);

        StartGame();
    }

    private void StartGame()
    {
        SceneManager.LoadScene("LobbyScene");
    }

    public IEnumerator DownloadRemoteAssets(object key)
    {
        var downloadSize = Addressables.GetDownloadSizeAsync(key);
        yield return downloadSize;
        long assetSize = downloadSize.Result;
        Addressables.Release(downloadSize);

        Debug.Log($"{key} 다운로드 시작 (크기: {FormatFileSize(assetSize)})");

        _downloadHandle = Addressables.DownloadDependenciesAsync(key, false);

        while (!_downloadHandle.IsDone)
        {
            float assetProgress = _downloadHandle.PercentComplete;
            float overallProgress = (_downloadedBytes + (long)(assetSize * assetProgress)) / (float)_totalDownloadSize;

            if (slider != null) slider.value = overallProgress;
            if (progressText != null) progressText.text = $"{(overallProgress * 100):F1}%";

            Debug.Log($"{key} 다운로드 진행률: {assetProgress * 100:F1}%, 전체 진행률: {overallProgress * 100:F1}%");

            yield return null;
        }

        if (_downloadHandle.Status == AsyncOperationStatus.Succeeded)
        {
            _downloadedBytes += assetSize;
            Debug.Log($"{key} 다운로드 완료!");
        }
        else
        {
            Debug.LogError($"{key} 다운로드 실패: {_downloadHandle.OperationException}");
        }

        Addressables.Release(_downloadHandle);
    }

    private string FormatFileSize(long bytes)
    {
        string[] suffixes = { "B", "KB", "MB", "GB", "TB" };
        int suffixIndex = 0;
        double size = bytes;

        while (size >= 1024 && suffixIndex < suffixes.Length - 1)
        {
            size /= 1024;
            suffixIndex++;
        }

        return $"{size:F2} {suffixes[suffixIndex]}";
    }

    private void OnDestroy()
    {
        if (_downloadHandle.IsValid())
            Addressables.Release(_downloadHandle);
    }
}