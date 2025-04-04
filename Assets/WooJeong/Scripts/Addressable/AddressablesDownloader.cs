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
            // īŻ�α� ������Ʈ�� �ʿ��� ���
            var updateOperation = Addressables.UpdateCatalogs(checkCatalogOperation.Result);
            yield return updateOperation;

            // �� ������ �ٿ�ε�
            yield return StartCoroutine(PreloadRemoteAssets());
        }
        else
        {
            Debug.Log("��� �������� �ֽ� �����Դϴ�. �ٿ�ε� �ʿ� ����.");
        }
        
        StartGame();
    }

    private void StartGame()
    {
        SceneManager.LoadScene("LobbyScene");
    }

    public IEnumerator PreloadRemoteAssets()
    {
        Debug.Log("����: ���� ���� �ٿ�ε�");

        _downloadHandle = Addressables.DownloadDependenciesAsync(labelToPreload, false);

        while (!_downloadHandle.IsDone)
        {
            float progress = _downloadHandle.PercentComplete;
            if (slider != null)
                slider.value = progress; // SetProgress(progress);

            Debug.Log($"�ٿ�ε� �����: {progress * 100}%");
            yield return null;
        }

        if (_downloadHandle.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log("���� ���� �ٿ�ε� �Ϸ�!");
            // ���� �޸� �ε�� LoadAssetAsync ���� �޼��带 ����� �� �߻�
        }
        else
        {
            Debug.LogError($"�ٿ�ε� ����: {_downloadHandle.OperationException}");
        }
    }

    private void OnDestroy()
    {
        if (_downloadHandle.IsValid())
            Addressables.Release(_downloadHandle);
    }
}