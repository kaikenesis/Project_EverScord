using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using EverScord.Character;
using EverScord.Weapons;
using UnityEngine.AddressableAssets;

namespace EverScord
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager instance;

        public string userName;
        public PlayerData userData;

        [SerializeField] private List<AssetReferenceGameObject> assetReferenceList;

        public List<PhotonView> playerPhotonViews { get; private set; }
        public BulletControl BulletsControl { get; private set; }

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

        private async void Init()
        {
            playerPhotonViews = new();
            BulletsControl = GameObject.FindGameObjectWithTag(ConstStrings.TAG_BULLETCONTROL).GetComponent<BulletControl>();

            foreach (AssetReference reference in assetReferenceList)
                await ResourceManager.Instance.CreatePool(reference.AssetGUID);
        }

        public void SetUserName(EJob curJob, ELevel curLevel)
        {
            PlayerPrefs.SetString("USERNAME", userName + "|" + curJob.ToString() + "|" + curLevel.ToString());
            PhotonNetwork.NickName = PlayerPrefs.GetString("USERNAME");
        }

        public void AddPlayerPhotonView(PhotonView view)
        {
            playerPhotonViews.Add(view);
        }

        public void AddPlayerControl(CharacterControl player)
        {
            playerDict[player.CharacterPhotonView.ViewID] = player;
        }
    }
}