using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using EverScord.Character;
using EverScord.Weapons;
using EverScord.Monster;

namespace EverScord
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager instance;

        public PlayerData userData;

        public List<PhotonView> playerPhotonViews   { get; private set; }
        public BulletControl BulletsControl         { get; private set; }
        public EnemyHitControl EnemyHitsControl     { get; private set; }
        public static int EnemyLayerNumber          { get; private set; }

        public static LayerMask GroundLayer => instance.groundLayer;
        public static LayerMask EnemyLayer => instance.enemyLayer;
        public static LayerMask PlayerLayer => instance.playerLayer;
        public static LayerMask OutlineLayer => instance.outlineLayer;
        public IDictionary<int, CharacterControl> PlayerDict => playerDict;

        [SerializeField] private LayerMask groundLayer, enemyLayer, playerLayer, outlineLayer;

        [SerializeField] private CostData costData;
        public CostData CostDatas
        {
            get { return costData; }
            private set { costData = value; }
        }

        [SerializeField] private FactorDatas[] factorDatas;
        public FactorDatas[] FactorDatas
        {
            get { return factorDatas; }
            private set { factorDatas = value; }
        }

        private IDictionary<int, CharacterControl> playerDict;

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
            playerDict          = new Dictionary<int, CharacterControl>();
            playerPhotonViews   = new();
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
            }
        }
    }
}