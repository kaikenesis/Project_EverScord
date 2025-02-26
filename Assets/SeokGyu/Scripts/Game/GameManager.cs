using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Photon.Pun;
using EverScord.Character;
using EverScord.Weapons;
using EverScord.Monster;
using EverScord.Effects;
using DG.Tweening;

namespace EverScord
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager instance;
        public const float GROUND_HEIGHT = 0f;
        private const float LOADSCREEN_DELAY = 3f;

        [SerializeField] private PlayerData playerData;
        public PlayerData PlayerData { get { return playerData; } }
        //public PlayerData userData;

        [SerializeField] private PhotonData photonData;
        public PhotonData PhotonData { get { return photonData; } }

        public List<PhotonView> playerPhotonViews               { get; private set; }
        public BulletControl BulletsControl                     { get; private set; }
        public EnemyHitControl EnemyHitsControl                 { get; private set; }
        public MonsterProjectileController ProjectileController { get; private set; }
        public LevelControl LevelController                     { get; private set; }
        public LoadingScreen LoadScreen                         { get; private set; }
        public static int EnemyLayerNumber                      { get; private set; }
        public static int PlayerLayerNumber                     { get; private set; }
        public static int CurrentStageIndex                     { get; private set; }
        public static bool IsLoadingLevel                       { get; private set; }

        public static LayerMask GroundLayer => instance.groundLayer;
        public static LayerMask EnemyLayer => instance.enemyLayer;
        public static LayerMask PlayerLayer => instance.playerLayer;
        public static LayerMask OutlineLayer => instance.outlineLayer;
        public static LayerMask RedOutlineLayer => instance.redOutlineLayer;
        public static BlinkEffectInfo HurtBlinkInfo => instance.hurtBlinkInfo;
        public static BlinkEffectInfo InvincibleBlinkInfo => instance.invincibleBlinkInfo;
        public IDictionary<int, CharacterControl> PlayerDict => playerDict;

        [SerializeField] private LayerMask groundLayer, enemyLayer, playerLayer, outlineLayer, redOutlineLayer;
        [SerializeField] private BlinkEffectInfo hurtBlinkInfo, invincibleBlinkInfo;
        [SerializeField] private List<StageInfo> stageInfos;
        [SerializeField] private CostData costData;

        public CostData CostDatas
        {
            get { return costData; }
            private set { costData = value; }
        }

        [SerializeField] private FactorData[] factorDatas;
        public FactorData[] FactorDatas
        {
            get { return factorDatas; }
            private set { factorDatas = value; }
        }

        [SerializeField] private InGameUIData inGameUIData;
        public InGameUIData InGameUIData
        {
            get { return inGameUIData; }
            private set { inGameUIData = value; }
        }

        [SerializeField] private ArmorData armorData;
        public ArmorData ArmorData
        {
            get { return armorData; }
            private set { armorData = value; }
        }

        private IDictionary<int, CharacterControl> playerDict;
        private WaitForSeconds waitLoadScreen;

        public static GameManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameManager();
                }
                return instance;
            }
        }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                Init();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Init()
        {
            EnemyLayerNumber    = Mathf.RoundToInt(Mathf.Log(EnemyLayer.value, 2));
            PlayerLayerNumber   = Mathf.RoundToInt(Mathf.Log(PlayerLayer.value, 2));
            playerDict          = new Dictionary<int, CharacterControl>();
            waitLoadScreen      = new WaitForSeconds(LOADSCREEN_DELAY);
            playerPhotonViews   = new();
            playerData.Initialize();

            CurrentStageIndex = -1;
            PhotonNetwork.AutomaticallySyncScene = true;
            photonData.Initialize();
        }

        public void UpdateUserName(string newName)
        {
            PhotonNetwork.NickName = newName;
        }

        public void AddPlayerPhotonView(PhotonView view)
        {
            playerPhotonViews.Add(view);
        }

        public void InitControl(object control)
        {
            if (control == null)
                return;
            
            switch (control)
            {
                case BulletControl bulletControl:
                    BulletsControl = bulletControl;
                    break;

                case EnemyHitControl enemyHitControl:
                    EnemyHitsControl = enemyHitControl;
                    break;

                case CharacterControl character:
                    playerDict[character.CharacterPhotonView.ViewID] = character;
                    break;

                case MonsterProjectileController projectileController:
                    ProjectileController = projectileController;
                    break;

                case LevelControl levelControl:
                    LevelController = levelControl;
                    break;

                case LoadingScreen loadScreen:
                    LoadScreen = loadScreen;
                    break;

                default:
                    break;
            }
        }

        public static void LoadLevel()
        {
            if (!PhotonNetwork.IsConnected || !PhotonNetwork.IsMasterClient)
                return;

            ++CurrentStageIndex;

            if (CurrentStageIndex >= Instance.stageInfos.Count)
            {
                Debug.LogWarning($"Invalid stage index : {CurrentStageIndex}");
                return;
            }

            Instance.StartCoroutine(Instance.LoadLevelAsync());
        }

        private IEnumerator LoadLevelAsync()
        {
            IsLoadingLevel = true;

            LoadScreen.CoverScreen();
            yield return new WaitForSeconds(1f);

            Camera.main.enabled = false;

            LoadScreen.ImageHub.SetActive(true);
            LoadScreen.ShowScreen();

            PhotonNetwork.LoadLevel(Instance.stageInfos[CurrentStageIndex].stageName);

            while (PhotonNetwork.LevelLoadingProgress < 0.98f)
            {
                LoadScreen.SetProgress(PhotonNetwork.LevelLoadingProgress);
                yield return null;
            }

            LoadScreen.SetProgress(1f);

            Instance.StartCoroutine(ExitLoadingScreen());
        }

        private IEnumerator ExitLoadingScreen()
        {
            yield return waitLoadScreen;

            LoadScreen.CoverScreen();
            yield return new WaitForSeconds(3f);

            LoadScreen.ShowScreen();
            LoadScreen.ImageHub.SetActive(false);

            IsLoadingLevel = false;
        }
    }
}

[System.Serializable]
public class StageInfo
{
    public string stageName;
    public AudioClip loopBgm;
}