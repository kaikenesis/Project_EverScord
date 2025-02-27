using System;
using UnityEngine;
using DG.Tweening;
using EverScord.Skill;
using EverScord.Character;
using System.Collections.Generic;
using Photon.Pun;
using System.Collections;

namespace EverScord
{
    public class LevelControl : MonoBehaviour
    {
        private const float DEFAULT_MAX_PROGRESS = 100f;
        private const float LOADSCREEN_DELAY = 3f;

        public static Action<float> OnProgressUpdated = delegate { };
        public static bool IsLoadingLevel { get; private set; }

        [SerializeField] private PortalControl portalControl;
        [SerializeField] private GameObject portal, groundCollider;
        [SerializeField] private float increaseAmount;
        [SerializeField] private float countdown;
        [SerializeField] private List<LevelInfo> levelList;

        private static WaitForSeconds waitLoadScreen;
        private float progress, maxProgress;

        void Awake()
        {
            GameManager.Instance.InitControl(this);
            waitLoadScreen = new WaitForSeconds(LOADSCREEN_DELAY);

            portalControl.gameObject.SetActive(false);
            portalControl.SetIsPortalOpened(false);
            portalControl.SetPortalCollider(false);
            portalControl.Init(countdown, TeleportPlayers);

            SetMaxProgress(DEFAULT_MAX_PROGRESS);

            OnProgressUpdated -= portalControl.TryOpenPortal;
            OnProgressUpdated += portalControl.TryOpenPortal;
        }

        void OnDisable()
        {
            OnProgressUpdated -= portalControl.TryOpenPortal;
        }

        public void SetMaxProgress(float amount)
        {
            maxProgress = amount;
        }

        public void IncreaseProgress()
        {
            progress = Mathf.Min(progress + increaseAmount, maxProgress);
            float currentProgress = progress / maxProgress;

            OnProgressUpdated?.Invoke(currentProgress);

            Debug.Log($"Current Level Progress: {progress}");
        }

        private void TeleportPlayers()
        {
            Debug.Log("Teleporting all players.");

            foreach (var kv in GameManager.Instance.PlayerDict)
            {
                CharacterControl player = kv.Value;

                // play effects

                // make player invisible

                PrepareNextLevel();
            }
        }

        private void PrepareNextLevel()
        {
            foreach (PhotonView view in GameManager.Instance.playerPhotonViews)
                view.gameObject.SetActive(false);

            levelList[GameManager.CurrentLevelIndex].Level.SetActive(false);
            GameManager.SetLevelIndex(GameManager.CurrentLevelIndex + 1);

            GameObject nextLevel = levelList[GameManager.CurrentLevelIndex].Level;
            nextLevel.SetActive(true);

            groundCollider.transform.position = nextLevel.transform.position;

            foreach (PhotonView view in GameManager.Instance.playerPhotonViews)
            {
                view.transform.position = nextLevel.transform.position;
                view.gameObject.SetActive(true);
            }
        }

        public static void LoadGameLevel()
        {
            if (!PhotonNetwork.IsConnected || !PhotonNetwork.IsMasterClient)
                return;

            GameManager.SetLevelIndex(0);
            GameManager.Instance.StartCoroutine(LoadLevelAsync(ConstStrings.SCENE_MAINGAME));
        }

        private static IEnumerator LoadLevelAsync(string levelName)
        {
            IsLoadingLevel = true;

            GameManager.Instance.LoadScreen.CoverScreen();
            yield return new WaitForSeconds(1f);

            GameManager.Instance.LoadScreen.ImageHub.SetActive(true);
            GameManager.Instance.LoadScreen.ShowScreen();

            PhotonNetwork.LoadLevel(levelName);

            while (PhotonNetwork.LevelLoadingProgress < 0.98f)
            {
                GameManager.Instance.LoadScreen.SetProgress(PhotonNetwork.LevelLoadingProgress);
                yield return null;
            }

            GameManager.Instance.LoadScreen.SetProgress(1f);
            GameManager.Instance.StartCoroutine(ExitLoadingScreen());
        }

        private static IEnumerator ExitLoadingScreen()
        {
            yield return waitLoadScreen;

            GameManager.Instance.LoadScreen.CoverScreen();
            yield return new WaitForSeconds(3f);

            GameManager.Instance.LoadScreen.ShowScreen();
            GameManager.Instance.LoadScreen.ImageHub.SetActive(false);

            IsLoadingLevel = false;
        }
    }
}

[System.Serializable]
public class LevelInfo
{
    public GameObject Level;
    public AudioClip LoopBgm;
}
