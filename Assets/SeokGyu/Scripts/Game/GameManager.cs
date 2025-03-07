using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using EverScord.Character;
using EverScord.Weapons;
using EverScord.Monster;
using EverScord.Effects;
using EverScord.Augment;

namespace EverScord
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager instance;
        public const float GROUND_HEIGHT = 0f;

        [SerializeField] private bool debugMode = false;
        [SerializeField] private PlayerData playerData;
        public PlayerData PlayerData { get { return playerData; } }

        [SerializeField] private PhotonData photonData;
        public PhotonData PhotonData { get { return photonData; } }

        [field: SerializeField] public LoadingScreen LoadScreen { get; private set; }
        public BulletControl BulletsControl                     { get; private set; }
        public EnemyHitControl EnemyHitsControl                 { get; private set; }
        public MonsterProjectileController ProjectileController { get; private set; }
        public LevelControl LevelController                     { get; private set; }
        public PortalControl PortalController                   { get; private set; }
        public AugmentPresenter AugmentControl                  { get; private set; }
        public static PhotonView View                           { get; private set; }
        public static int EnemyLayerNumber                      { get; private set; }
        public static int PlayerLayerNumber                     { get; private set; }
        public static int CurrentLevelIndex                     { get; private set; }

        public static LayerMask GroundLayer => instance.groundLayer;
        public static LayerMask EnemyLayer => instance.enemyLayer;
        public static LayerMask PlayerLayer => instance.playerLayer;
        public static LayerMask OutlineLayer => instance.outlineLayer;
        public static LayerMask RedOutlineLayer => instance.redOutlineLayer;
        public static BlinkEffectInfo HurtBlinkInfo => instance.hurtBlinkInfo;
        public static BlinkEffectInfo InvincibleBlinkInfo => instance.invincibleBlinkInfo;
        public static BlinkEffectInfo StunnedBlinkInfo => instance.stunnedBlinkInfo;
        public IDictionary<int, CharacterControl> PlayerDict => playerDict;

        [SerializeField] private LayerMask groundLayer, enemyLayer, playerLayer, outlineLayer, redOutlineLayer;
        [SerializeField] private BlinkEffectInfo hurtBlinkInfo, invincibleBlinkInfo, stunnedBlinkInfo;
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

        [SerializeField] private MinimapData minimapData;
        public MinimapData MinimapData
        {
            get { return minimapData; }
            private set { minimapData = value; }
        }

        [SerializeField] private GameMode gameMode;
        public GameMode GameMode
        {
            get { return gameMode; }
        }

        [SerializeField] private PointMarkData pointMarkData;
        public PointMarkData PointMarkData
        {
            get { return pointMarkData; }
        }

        private IDictionary<int, CharacterControl> playerDict;

        public static GameManager Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject manager = new GameObject("_GameManager");
                    instance = manager.AddComponent<GameManager>();
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
            View                = gameObject.AddComponent<PhotonView>();
            EnemyLayerNumber    = Mathf.RoundToInt(Mathf.Log(EnemyLayer.value, 2));
            PlayerLayerNumber   = Mathf.RoundToInt(Mathf.Log(PlayerLayer.value, 2));
            playerDict          = new Dictionary<int, CharacterControl>();

            View.ViewID = 999;
            CurrentLevelIndex = -1;
            PhotonNetwork.AutomaticallySyncScene = true;

            playerData.Initialize();
            photonData.Initialize();
            gameMode.Initialize();
        }

        public void UpdateUserName(string newName)
        {
            PhotonNetwork.NickName = newName;
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

                case PortalControl portalControl:
                    PortalController = portalControl;
                    break;

                case AugmentPresenter augmentPresenter:
                    AugmentControl = augmentPresenter;
                    break;

                default:
                    break;
            }
        }

        public static void SetLevelIndex(int index)
        {
            CurrentLevelIndex = index;
        }

        public static void ResetPlayerInfo()
        {
            Instance.PlayerDict.Clear();
        }

        [PunRPC]
        public void ReviveAllPlayers()
        {
            foreach (var kv in PlayerDict)
            {
                CharacterControl player = kv.Value;

                if (!player.IsDead)
                    continue;

                player.StartCoroutine(player.HandleRevival());
            }
        }

        [PunRPC]
        public void SyncLoadGameLevel()
        {
            SetLevelIndex(0);
            Instance.StartCoroutine(LevelControl.LoadLevelAsync(ConstStrings.SCENE_MAINGAME));
        }

        [PunRPC]
        public void PrepareNextLevel()
        {
            StartCoroutine(LevelController.PrepareNextLevel());
        }

        private void OnGUI()
        {
            if (debugMode == true)
            {
                if (GUI.Button(new Rect(200, 0, 120, 60), "Show Me The Money"))
                {
                    playerData.IncreaseMoney(10000);
                }

                if (GUI.Button(new Rect(200, 70, 120, 60), "Play"))
                {
                    //PhotonNetwork.LoadLevel("PhotonTestPlay");
                    LevelControl.LoadGameLevel();
                }

                if (GUI.Button(new Rect(200, 140, 120, 60), "Go To LobbyScene"))
                {
                    if(PhotonNetwork.IsMasterClient)
                    {
                        PhotonNetwork.LoadLevel("PhotonTestLobby");
                        ResourceManager.ClearAllPools();
                    }
                }
            }
        }
    }
}