using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;
using Photon.Pun;
using EverScord.Effects;

namespace EverScord
{
    public class LevelControl : MonoBehaviour
    {
        private const float DEFAULT_MAX_PROGRESS = 100f;
        private const float LOADSCREEN_DELAY = 3f;
        private const float STAGE_TRANSITION_DELAY = 2f;
        private const float STAGE_TRANSITION_FADE_DELAY = 3f;

        public static Action<float> OnProgressUpdated = delegate { };
        public static bool IsLoadingLevel { get; private set; }
        private static WaitForSeconds waitLoadScreen, waitStageTransition, waitStageFade;
        private static WaitForSeconds waitOneSec = new WaitForSeconds(1f);
        private static WaitForSeconds waitPointOne = new WaitForSeconds(0.1f); 

        [SerializeField] private PortalControl portalControl;
        [SerializeField] private GameObject portal, groundCollider;
        [SerializeField] private float increaseAmount;
        [SerializeField] private float countdown;
        [SerializeField] private List<LevelInfo> levelList;

        private float progress, maxProgress;

        void Awake()
        {
            GameManager.Instance.InitControl(this);

            waitLoadScreen      = new WaitForSeconds(LOADSCREEN_DELAY);
            waitStageTransition = new WaitForSeconds(STAGE_TRANSITION_DELAY);
            waitStageFade       = new WaitForSeconds(STAGE_TRANSITION_FADE_DELAY);

            portalControl.Init(countdown, NotifyTeleport);
            portalControl.ResetPortal();

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

        public void IncreaseProgress(MonsterType monsterType)
        {
            progress = Mathf.Min(progress + increaseAmount, maxProgress);
            float currentProgress = progress / maxProgress;

            OnProgressUpdated?.Invoke(currentProgress);
        }

        private void ResetProgress()
        {
            progress = 0f;
        }

        private void NotifyTeleport()
        {
            if (!PhotonNetwork.IsConnected || !PhotonNetwork.IsMasterClient)
                return;

            GameManager.View.RPC(nameof(GameManager.Instance.PrepareNextLevel), RpcTarget.All);
        }
        
        public IEnumerator PrepareNextLevel()
        {
            foreach (var player in GameManager.Instance.PlayerDict.Values)
                player.SetState(Character.SetCharState.ADD, Character.CharState.TELEPORTING);

            yield return waitStageTransition;

            portalControl.PlayWarpEffect(true);
            yield return waitStageTransition;

            var beamEffect = ResourceManager.Instance.GetAsset<GameObject>(AssetReferenceManager.ReviveBeam_ID);

            foreach (var player in GameManager.Instance.PlayerDict.Values)
            {
                player.SetActive(false);
                Instantiate(beamEffect, player.PlayerTransform.position, Quaternion.identity);
                yield return waitPointOne;
            }
            yield return waitStageTransition;

            GameManager.Instance.LoadScreen.CoverScreen();
            yield return waitStageFade;

            SetNextLevel(out GameObject nextLevel);
            portalControl.MovePosition(nextLevel.transform.position);

            foreach (var player in GameManager.Instance.PlayerDict.Values)
                player.Teleport(portalControl.GetRandomPosition());

            yield return new WaitForSeconds(0.8f);

            GameManager.Instance.LoadScreen.ShowScreen();
            portalControl.PlayWarpEffect(false);
            yield return waitStageTransition;

            foreach (var player in GameManager.Instance.PlayerDict.Values)
            {
                Instantiate(beamEffect, player.PlayerTransform.position, Quaternion.identity);
                player.SetActive(true);
                player.SetState(Character.SetCharState.REMOVE, Character.CharState.TELEPORTING);
                yield return waitPointOne;
            }

            ResetProgress();
            portalControl.ClosePortal();
        }

        private void SetNextLevel(out GameObject nextLevel)
        {
            levelList[GameManager.CurrentLevelIndex].Level.SetActive(false);
            GameManager.SetLevelIndex(GameManager.CurrentLevelIndex + 1);

            nextLevel = levelList[GameManager.CurrentLevelIndex].Level;
            groundCollider.transform.position = nextLevel.transform.position;
            nextLevel.SetActive(true);
        }

        public static void LoadGameLevel()
        {            
            if (!PhotonNetwork.IsConnected)
                return;
            
            GameManager.View.RPC(nameof(GameManager.Instance.SyncLoadGameLevel), RpcTarget.All);
        }

        public static IEnumerator LoadLevelAsync(string levelName)
        {
            IsLoadingLevel = true;

            GameManager.Instance.LoadScreen.CoverScreen();
            yield return waitOneSec;

            GameManager.Instance.LoadScreen.ImageHub.SetActive(true);
            GameManager.Instance.LoadScreen.ShowScreen();

            if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient)
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
