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
    [SerializeField] private TextMeshProUGUI sizeInfoText; // �ٿ�ε� ũ�� ������ ǥ���� Text
    [SerializeField] private TextMeshProUGUI progressText; // �ٿ�ε� ������� ǥ���� Text
    [SerializeField] private Button downloadButton; // �ٿ�ε� ���� ��ư

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
        // UI �ʱ�ȭ
        if (slider != null) slider.value = 0;
        if (sizeInfoText != null) sizeInfoText.text = "������Ʈ Ȯ�� ��...";
        if (progressText != null) progressText.text = "";

        var checkCatalogOperation = Addressables.CheckForCatalogUpdates();
        yield return checkCatalogOperation;
        
        if (checkCatalogOperation.Result.Count > 0)
        {
            // īŻ�α� ������Ʈ�� �ʿ��� ���
            if (sizeInfoText != null) sizeInfoText.text = "īŻ�α� ������Ʈ ��...";

            var updateOperation = Addressables.UpdateCatalogs(checkCatalogOperation.Result);
            yield return updateOperation;

            // �ٿ�ε尡 �ʿ��� Ű�� �� ũ�� ���
            _keysToDownload.Clear();
            _totalDownloadSize = 0;

            if (sizeInfoText != null) sizeInfoText.text = "�ٿ�ε� ũ�� ��� ��...";

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
                        Debug.Log($"�ٿ�ε� �ʿ��� Ű: {key}, ũ��: {downloadSize.Result} ����Ʈ");
                    }

                    Addressables.Release(downloadSize);
                }
            }

            // �� �ٿ�ε� ũ�� ǥ��
            string sizeText = FormatFileSize(_totalDownloadSize);
            if (sizeInfoText != null) sizeInfoText.text = $"�ٿ�ε� �ʿ�: {sizeText}";

            // ����ڿ��� �ٿ�ε� ���� �˸� (�ʿ��� ��� ���⿡ Ȯ�� ���̾�α� �߰� ����)
            Debug.Log($"�� {_keysToDownload.Count}�� ����, {sizeText} �ٿ�ε� �ʿ�");

            downloadButton.enabled = true;

            //// �� Ű �ٿ�ε�
            //_downloadedBytes = 0;
            //foreach (var key in _keysToDownload)
            //{
            //    yield return StartCoroutine(DownloadRemoteAssets(key));
            //}

            //if (sizeInfoText != null) sizeInfoText.text = "�ٿ�ε� �Ϸ�!";
            //if (progressText != null) progressText.text = "100%";

            //Addressables.Release(updateOperation);
        }
        else
        {
            Debug.Log("��� �������� �ֽ� �����Դϴ�. �ٿ�ε� �ʿ� ����.");
            if (sizeInfoText != null) sizeInfoText.text = "��� �������� �ֽ� �����Դϴ�.";
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

        if (sizeInfoText != null) sizeInfoText.text = "�ٿ�ε� �Ϸ�!";
        if (progressText != null) progressText.text = "100%";
        
        // �ٿ�ε� �Ϸ� �� ��� ��� (����ڰ� �Ϸ� �޽����� �� �� �ֵ���)
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

        Debug.Log($"{key} �ٿ�ε� ���� (ũ��: {FormatFileSize(assetSize)})");

        _downloadHandle = Addressables.DownloadDependenciesAsync(key, false);

        while (!_downloadHandle.IsDone)
        {
            float assetProgress = _downloadHandle.PercentComplete;
            float overallProgress = (_downloadedBytes + (long)(assetSize * assetProgress)) / (float)_totalDownloadSize;

            if (slider != null) slider.value = overallProgress;
            if (progressText != null) progressText.text = $"{(overallProgress * 100):F1}%";

            Debug.Log($"{key} �ٿ�ε� �����: {assetProgress * 100:F1}%, ��ü �����: {overallProgress * 100:F1}%");

            yield return null;
        }

        if (_downloadHandle.Status == AsyncOperationStatus.Succeeded)
        {
            _downloadedBytes += assetSize;
            Debug.Log($"{key} �ٿ�ε� �Ϸ�!");
        }
        else
        {
            Debug.LogError($"{key} �ٿ�ε� ����: {_downloadHandle.OperationException}");
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