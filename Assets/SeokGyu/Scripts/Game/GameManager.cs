using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using EverScord.Character;
using EverScord.Weapons;
using EverScord.Monster;
using Photon.Realtime;

namespace EverScord
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager instance;

        public PlayerData userData;

        public List<PhotonView> playerPhotonViews   { get; private set; }
        public BulletControl BulletsControl         { get; private set; }
        public EnemyHitControl EnemyHitsControl     { get; private set; }

        private IDictionary<int, CharacterControl> playerDict = new Dictionary<int, CharacterControl>();
        public IDictionary<int, CharacterControl> PlayerDict => playerDict;

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
            playerPhotonViews = new();
        }

        public void UpdateUserName(string newName)
        {
            PhotonNetwork.NickName = newName;
        }

        public void AddPlayerPhotonView(PhotonView view)
        {
            playerPhotonViews.Add(view);
        }

        public void AddPlayerControl(CharacterControl player)
        {
            playerDict[player.CharacterPhotonView.ViewID] = player;
        }

        public void SetBulletControl(BulletControl bulletControl)
        {
            BulletsControl = bulletControl;
        }

        public void InitControl<T>(T control)
        {
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

                default:
                    break;
            }
        }
    }
}