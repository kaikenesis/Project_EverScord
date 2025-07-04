using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;
using Photon.Pun;
using EverScord.Effects;
using EverScord.GameCamera;

namespace EverScord
{
    public class LevelControl : MonoBehaviour
    {
        private const float MAX_PROGRESS = 100f;
        private const float LOADSCREEN_DELAY = 3f;
        private const float STAGE_TRANSITION_DELAY = 2f;
        private const float STAGE_TRANSITION_FADE_DELAY = 3f;

        public static Action<float> OnProgressUpdated = delegate { };
        public static Action<int, bool> OnLevelUpdated = delegate { };
        public static Action OnLoadComplete = delegate { };

        public static PhotonView View { get; private set; }
        public static bool IsLoadingLevel { get; private set; }
        public static bool IsBossMode { get; private set; }
        public static bool IsReturningToLobby { get; private set; }

        public PortalControl PortalController => portalControl;
        public float CurrentProgress => progress / Mathf.Max(0.001f, maxProgress);
        public static bool IsLevelCompleted => progress >= maxProgress;
        public static float MaxProgress => maxProgress;
        public int MaxLevelIndex => levelList.Count - 1;

        private static WaitForSeconds waitLoadScreen, waitStageTransition, waitStageFade;
        private static WaitForSeconds waitOneSec = new WaitForSeconds(1f);
        private static WaitForSeconds waitPointOne = new WaitForSeconds(0.1f);
        private static float progress, maxProgress;

        [SerializeField] private UIProgress progressUI;
        [SerializeField] private PortalControl portalControl;
        [SerializeField] private GameObject portal, groundCollider;
        [SerializeField] private float countdown;
        [SerializeField] private Color bossHpBarColor;
        [SerializeField] private List<LevelInfo> levelList;

        private IDictionary<MonsterType, float> increaseDict = new Dictionary<MonsterType, float>
        {
            {MonsterType.SMALL, 2f},
            {MonsterType.MEDIUM, 5f},
            {MonsterType.LARGE, 25f}
        };

        void Awake()
        {
            GameManager.Instance.InitControl(this);

            View = GetComponent<PhotonView>();

            waitLoadScreen      = new WaitForSeconds(LOADSCREEN_DELAY);
            waitStageTransition = new WaitForSeconds(STAGE_TRANSITION_DELAY);
            waitStageFade       = new WaitForSeconds(STAGE_TRANSITION_FADE_DELAY);

            portalControl.Init(countdown, NotifyTeleport);
            portalControl.ResetPortal();

            IsBossMode = false;
            IsReturningToLobby = false;
            SetMaxProgress(MAX_PROGRESS);

            progress = 0f;

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

        public void ChangeCurrentProgress(float changeAmount)
        {
            progress = Mathf.Clamp(progress + changeAmount, 0, maxProgress);
            OnProgressUpdated?.Invoke(CurrentProgress);
        }

        public void SetCurrentProgress(float amount)
        {
            progress = Mathf.Clamp(amount, 0, maxProgress);
            OnProgressUpdated?.Invoke(CurrentProgress);
        }

        public static void SyncChangeCurrentProgress(float changeAmount)
        {
            if (PhotonNetwork.IsConnected)
                View.RPC(nameof(SyncProgress), RpcTarget.All, changeAmount);
        }

        [PunRPC]
        public void SyncProgress(float changeAmount)
        {
            ChangeCurrentProgress(changeAmount);
        }

        public void IncreaseMonsterProgress(MonsterType monsterType)
        {
            ChangeCurrentProgress(increaseDict[monsterType]);
        }

        public void IncreaseBossProgress(BossRPC boss)
        {
            int currentPhase = boss.Phase;
            float bossMaxHP = boss.MaxHP;
            float bossCurrentHP = boss.HP;

            float amount = bossCurrentHP / bossMaxHP * maxProgress;
            SetCurrentProgress(amount);
        }

        public void ResetProgress()
        {
            progress = 0f;
            OnProgressUpdated?.Invoke(0);
            IsBossMode = false;
            progressUI.ChangeFillColor(null);
        }

        public void SetBossMode(bool state)
        {
            IsBossMode = state;

            if (IsBossMode)
                progressUI.ChangeFillColor(bossHpBarColor);
        }

        private void NotifyTeleport()
        {
            if (!PhotonNetwork.IsConnected || !PhotonNetwork.IsMasterClient)
                return;

            View.RPC(nameof(SyncPrepareNextLevel), RpcTarget.All);
        }
        
        public IEnumerator PrepareNextLevel()
        {
            SoundManager.Instance.PlaySound(ConstStrings.SFX_PORTAL_LOOP);

            foreach (var player in GameManager.Instance.PlayerDict.Values)
            {
                player.SetState(Character.SetCharState.ADD, Character.CharState.TELEPORTING);
                player.AnimationControl.Rotate(false);
                player.PlayerWeapon.SetShootingStance(player, false, true);
            }

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

            LoadingScreen.CoverScreen();
            yield return waitStageFade;

            portalControl.ScanEffect.gameObject.SetActive(false);

            bool bCoverScreen = true;
            OnLevelUpdated?.Invoke(GameManager.CurrentLevelIndex + 1, bCoverScreen);

            SetNextLevel(out GameObject nextLevel);
            portalControl.MovePosition(nextLevel.transform.position);

            foreach (var player in GameManager.Instance.PlayerDict.Values)
                player.Teleport(portalControl.GetRandomPosition());

            yield return new WaitForSeconds(0.8f);

            LoadingScreen.ShowScreen();
            portalControl.PlayWarpEffect(false);
            SoundManager.Instance.PlaySound(ConstStrings.SFX_PORTAL_ARRIVAL);

            yield return waitStageTransition;

            bCoverScreen = false;
            OnLevelUpdated?.Invoke(GameManager.CurrentLevelIndex + 1, bCoverScreen);

            foreach (var player in GameManager.Instance.PlayerDict.Values)
            {
                player.SetActive(true);
                player.SetState(Character.SetCharState.REMOVE, Character.CharState.TELEPORTING);
                player.SetState(Character.SetCharState.REMOVE, Character.CharState.INVINCIBLE);
                Instantiate(beamEffect, player.PlayerTransform.position, Quaternion.identity);
                yield return waitPointOne;
            }

            SoundManager.Instance.PlaySound(ConstStrings.SFX_PORTAL_END);

            ResetProgress();
            portalControl.ClosePortal();
        }

        [PunRPC]
        public void SyncPrepareNextLevel()
        {
            StartCoroutine(PrepareNextLevel());
        }

        private void SetNextLevel(out GameObject nextLevel)
        {
            Debug.Log(GameManager.CurrentLevelIndex);
            levelList[GameManager.CurrentLevelIndex].Level.SetActive(false);
            GameManager.SetLevelIndex(GameManager.CurrentLevelIndex + 1);

            nextLevel = levelList[GameManager.CurrentLevelIndex].Level;
            groundCollider.transform.position = nextLevel.transform.position;
            nextLevel.SetActive(true);
        }

        public GameObject SetNextLevel(int level)
        {
            EnableLevel(GameManager.CurrentLevelIndex, false);
            GameManager.SetLevelIndex(level);

            groundCollider.transform.position = GetCurrentLevel().transform.position;
            EnableLevel(GameManager.CurrentLevelIndex, true);

            return GetCurrentLevel();
        }

        public void EnableLevel(int levelIndex, bool state)
        {
            if (levelIndex < 0 || levelIndex >= levelList.Count)
                return;

            levelList[levelIndex].Level.SetActive(state);
        }

        public GameObject GetCurrentLevel()
        {
            return levelList[GameManager.CurrentLevelIndex].Level;
        }

        public static void LoadGameLevel()
        {            
            if (!PhotonNetwork.IsConnected)
                return;

            GameManager.View.RPC(nameof(GameManager.SyncLoadGameLevel), RpcTarget.All);
        }

        [PunRPC]
        public void SyncReturnToLobby()
        {
            if (IsReturningToLobby)
                return;

            IsReturningToLobby = true;
            View.RPC(nameof(ReturnEveryoneToLobby), RpcTarget.All);
        }

        [PunRPC]
        public void ReturnEveryoneToLobby()
        {
            ReturnToLobby();
        }

        public static void ReturnToLobby()
        {
            GameManager.ResetGame();
            LoadScene(ConstStrings.SCENE_LOBBY);

            OnLoadComplete -= PlayLobbyBGM;
            OnLoadComplete += PlayLobbyBGM;
        }

        public static void LoadScene(string sceneName)
        {
            GameManager.Instance.StartCoroutine(LoadSceneAsync(sceneName));
        }

        public static IEnumerator LoadSceneAsync(string sceneName)
        {
            IsLoadingLevel = true;

            LoadingScreen.CoverScreen();
            yield return waitOneSec;

            GameManager.Instance.LoadScreen.ResetProgress();
            GameManager.Instance.LoadScreen.ImageHub.SetActive(true);
            LoadingScreen.ShowScreen();

            if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient)
                PhotonNetwork.LoadLevel(sceneName);

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

            LoadingScreen.CoverScreen();
            yield return new WaitForSeconds(3f);

            LoadingScreen.ShowScreen();
            GameManager.Instance.LoadScreen.ImageHub.SetActive(false);

            IsLoadingLevel = false;
            OnLoadComplete?.Invoke();
        }

        private static void PlayLobbyBGM()
        {
            SoundManager.Instance.PlayBGM("LobbyBGM");
            OnLoadComplete -= PlayLobbyBGM;
        }
    }
}

[System.Serializable]
public class LevelInfo
{
    public GameObject Level;
    public AudioClip LoopBgm;
}
