using System.Collections;
using System.Linq;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using EverScord.Character;
using DG.Tweening;

namespace EverScord.UI
{
    public class ResultPresenter : MonoBehaviour
    {
        private const float SHOW_RESULT_INTERVAL = 0.1f;

        [SerializeField] private GameObject uiHub, ingameUiHub;
        [SerializeField] private PhotonView photonView;
        [SerializeField] private List<ResultUI> resultUIList;
        [SerializeField] private GameObject nedPrefab, uniPrefab, usPrefab;
        [SerializeField] private List<Transform> positionList;
        [SerializeField] private DOTweenAnimation buttonTween, titleTween;

        private static int readyPlayerCount = 0;
        
        void Awake()
        {
            GameManager.Instance.InitControl(this);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F4))
            {
                if (PhotonNetwork.IsConnected)
                    photonView.RPC(nameof(SyncPlayerResults), RpcTarget.All);
            }

            if (Input.GetKeyDown(KeyCode.F5))
            {
                StartCoroutine(ShowResults());
            }
        }

        public void TransitionResults()
        {
            photonView.RPC(nameof(SyncPlayerResults), RpcTarget.All);
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
                resultUIList[i].Init(player.KillCount, player.DealtDamage, player.DealtHeal, player.Nickname);
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

        public IEnumerator ShowResults()
        {
            PlayerUI.SetCursor(CursorType.UIFOCUS);

            ingameUiHub.SetActive(false); ////
            uiHub.SetActive(true);

            buttonTween.DORewind();
            buttonTween.DOPlay();

            titleTween.DORewind();
            titleTween.DOPlay();

            yield return new WaitForSeconds(0.5f);

            for (int i = 0; i < resultUIList.Count; i++)
                resultUIList[i].gameObject.SetActive(false);

            for (int i = 0; i < resultUIList.Count; i++)
            {
                resultUIList[i].gameObject.SetActive(true);
                resultUIList[i].PlayTween();
                yield return new WaitForSeconds(SHOW_RESULT_INTERVAL);
            }
        }

        public void IncreaseReadyCount()
        {
            ++readyPlayerCount;
            if (readyPlayerCount == GameManager.Instance.PlayerDict.Count)
            {
                readyPlayerCount = 0;

                if (gameObject.activeSelf)
                {
                    SetupUI();
                    StartCoroutine(ShowResults());
                }
            }
        }

        [PunRPC]
        private void SyncPlayerResults()
        {
            CharacterControl player = CharacterControl.CurrentClientCharacter;
            PhotonView photonView = player.CharacterPhotonView;

            photonView.RPC(
                nameof(player.SyncPlayerResult),
                RpcTarget.All,
                player.KillCount,
                player.DealtDamage,
                player.DealtHeal,
                PhotonNetwork.NickName
            );
        }
    }
}
