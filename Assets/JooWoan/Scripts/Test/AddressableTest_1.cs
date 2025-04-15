using EverScord;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class AddressableTest_1 : MonoBehaviour
{
    [SerializeField] private AssetReference objectRef, musicRef;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private string labelName;

    [SerializeField] private GameObject updateNoticePanel;
    [SerializeField] private GameObject updateProgressBar;
    [SerializeField] private Image updateProgressFill;
    [SerializeField] private TextMeshProUGUI sizeText, progressText;

    private AsyncOperationHandle handle;

    private void Start()
    {
        updateNoticePanel.SetActive(false);
        updateProgressBar.SetActive(false);
        CheckUpdate();
    }

    private IEnumerator CheckUpdate()
    {
        AsyncOperationHandle<long> getDownloadSize = Addressables.GetDownloadSizeAsync(labelName);
        yield return getDownloadSize;

        // Download size is larger than zero
        if (getDownloadSize.Result > 0)
        {
            updateNoticePanel.SetActive(true);
            sizeText.text = $"{getDownloadSize.Result} Byte";
        }
        else
            StartGame();

        // Release memory
        Addressables.Release(getDownloadSize);
    }

    private IEnumerator StartUpdate()
    {
        // Download label
        handle = Addressables.DownloadDependenciesAsync(labelName);
        StartCoroutine(ShowUpdateProgress());

        yield return handle;
        yield return new WaitForSeconds(1f);

        updateNoticePanel.SetActive(false);
        updateProgressBar.SetActive(false);

        StartGame();
        Addressables.Release(handle);
    }

    private IEnumerator ShowUpdateProgress()
    {
        updateProgressBar.SetActive(true);
        yield return new WaitUntil(() => handle.IsValid());

        while (handle.PercentComplete < 1)
        {
            updateProgressFill.fillAmount = handle.PercentComplete;
            progressText.text = $"{handle.PercentComplete * 100:F2}%";

            yield return null;
        }

        updateProgressFill.fillAmount = 1;
        progressText.text = "100%";
    }

    private void StartGame()
    {
        CreateObject();
        PlayMusic();
    }

    public void CreateObject()
    {
        Addressables.LoadAssetAsync<GameObject>(objectRef).Completed += (op) =>
        {
            if (op.Status != AsyncOperationStatus.Succeeded)
                return;

            Instantiate(op.Result, transform.position, Quaternion.identity);
        };
    }

    public void PlayMusic()
    {
        Addressables.LoadAssetAsync<AudioClip>(musicRef).Completed += (op) =>
        {
            if (op.Status != AsyncOperationStatus.Succeeded)
                return;

            audioSource.clip = op.Result;
            audioSource.Play();
        };
    }
}
