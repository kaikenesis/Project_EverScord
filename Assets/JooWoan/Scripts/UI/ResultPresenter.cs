using EverScord.Character;
using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EverScord.UI
{
    public class ResultPresenter : MonoBehaviour
    {
        [SerializeField] private GameObject uiHub, ingameUiHub;
        [SerializeField] private PhotonView photonView;
        [SerializeField] private List<ResultUI> resultUIList;
        [SerializeField] private GameObject nedPrefab, uniPrefab, usPrefab;
        [SerializeField] private List<Transform> positionList;

        private static int readyPlayerCount = 0;
        
        void Awake()
        {
            GameManager.Instance.InitControl(this);
            SetResultUI(false);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F4))
            {
                if (PhotonNetwork.IsConnected)
                    photonView.RPC(nameof(SyncPlayerResults), RpcTarget.All);
            }
        }

        private void SetupUI()
        {
            List<CharacterControl> playerList = GameManager.Instance.PlayerDict.Values.ToList();
            int currentClientIndex = 0;

            for (int i = 0; i < playerList.Count; i++)
            {
                if (playerList[i] == CharacterControl.CurrentClientCharacter)
                {
                    currentClientIndex = i;
                    break;
                }
            }

            var temp = playerList[currentClientIndex];
            playerList.RemoveAt(currentClientIndex);
            playerList.Add(temp);

            for (int i = 0; i < playerList.Count; i++)
            {
                if (i >= resultUIList.Count)
                    continue;

                CharacterControl player = playerList[i];
                SpawnPlayerPrefab(player.CharacterType, i);
                resultUIList[i].Init(player.KillCount, player.DealtDamage, player.DealtHeal);
            }
        }

        private void SpawnPlayerPrefab(PlayerData.ECharacter type, int index)
        {
            GameObject spawnedPrefab;
            Transform spawnPoint = positionList[index];

            switch (type)
            {
                case PlayerData.ECharacter.Uni:
                    spawnedPrefab = Instantiate(uniPrefab, spawnPoint.parent);
                    break;

                case PlayerData.ECharacter.Us:
                    spawnedPrefab = Instantiate(usPrefab, spawnPoint.parent);
                    break;

                default:
                    spawnedPrefab = Instantiate(nedPrefab, spawnPoint.parent);
                    break;
            }

            spawnedPrefab.transform.localPosition = spawnPoint.transform.localPosition;
            spawnedPrefab.transform.localRotation = spawnPoint.transform.localRotation;
        }

        private void SetResultUI(bool state)
        {
            for (int i = 0; i < resultUIList.Count; i++)
                resultUIList[i].gameObject.SetActive(state);
        }

        public void ShowResults()
        {
            SetupUI();
            ingameUiHub.SetActive(false);
            uiHub.SetActive(true);
            SetResultUI(true);
        }

        public void IncreaseReadyCount()
        {
            ++readyPlayerCount;
            if (readyPlayerCount == GameManager.Instance.PlayerDict.Count)
            {
                readyPlayerCount = 0;
                ShowResults();
            }
        }

        [PunRPC]
        private void SyncPlayerResults()
        {
            CharacterControl player = CharacterControl.CurrentClientCharacter;
            PhotonView photonView = player.CharacterPhotonView;
            photonView.RPC(nameof(player.SyncPlayerResult), RpcTarget.All, player.KillCount, player.DealtDamage, player.DealtHeal);
        }
    }
}
