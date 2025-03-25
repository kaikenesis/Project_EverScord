using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine;
using DG.Tweening;
using Photon.Pun;
using EverScord.Character;
using EverScord.GameCamera;
using UnityEngine.UI;
using TMPro;

namespace EverScord.UI
{
    public class ResultPresenter : MonoBehaviour
    {
        private const float SHOW_RESULT_INTERVAL = 0.1f;
        private static int readyPlayerCount = 0;

        [SerializeField] private GameObject uiHub, victoryText, defeatText;
        [SerializeField] private List<GameObject> ingameUIList;
        [SerializeField] private PhotonView photonView;
        [SerializeField] private List<ResultUI> resultUIList;
        [SerializeField] private GameObject nedPrefab, uniPrefab, usPrefab;
        [SerializeField] private List<Transform> positionList;
        [SerializeField] private DOTweenAnimation buttonTween1, buttonTween2, titleTween, blackTween;
        [SerializeField] private TextMeshProUGUI returnCountText;
        [SerializeField] private Button lobbyButton;
        [SerializeField] private Color readyColor;

        private int[] rewardMoneyList = { 30, 60, 90, 120 };
        private List<int> returnRequestList;

        private DepthOfField depthOfField;
        private bool isVictory;

        void Awake()
        {
            GameManager.Instance.InitControl(this);

            uiHub.SetActive(false);
            blackTween.gameObject.SetActive(true);

            readyPlayerCount = 0;
            isVictory = false;
            returnRequestList = new();

            lobbyButton.onClick.AddListener(RequestReturnToLobby);
        }

        void Start()
        {
            InitDepthOfField();
        }

        void OnDisable()
        {
            lobbyButton.onClick.RemoveListener(RequestReturnToLobby);

            if (depthOfField)
            {
                depthOfField.focusDistance.value = 1f;
                depthOfField.active = false;
            }
        }

        private void ButtonSound(bool isEveryoneReady)
        {
            if (!isEveryoneReady)
                SoundManager.Instance.PlaySound(ConstStrings.SFX_BUTTON_2);
            else
                SoundManager.Instance.PlaySound(ConstStrings.SFX_BUTTON);
        }

        private void InitDepthOfField()
        {
            Transform cameraRoot = CharacterCamera.Root;
            Volume volume = cameraRoot.GetComponent<Volume>();

            if (volume.profile.TryGet<DepthOfField>(out var dof))
                depthOfField = dof;
        }

        public void TransitionResults(bool isVictory)
        {
            this.isVictory = isVictory;
            SyncPlayerResults();
            EarnRewardMoney();
        }

        private void EarnRewardMoney()
        {
            int totalReward = 0;

            for (int i = 0; i < GameManager.CurrentLevelIndex; i++)
                totalReward += rewardMoneyList[i];

            GameManager.Instance.UpdateMoney(totalReward);
        }

        private void SetupUI()
        {
            for (int i = 0; i < ingameUIList.Count; i++)
                ingameUIList[i].SetActive(false);

            GameManager.Instance.GameOverController.EnableUI(false);
            GameManager.Instance.LevelController.GetCurrentLevel().gameObject.SetActive(false);
            
            PlayerUI.SetCursor(CursorType.UIFOCUS);
            uiHub.SetActive(true);

            List<CharacterControl> playerList = GameManager.Instance.PlayerDict.Values.ToList();
            int currentClientIndex = 0;

            for (int i = 0; i < playerList.Count; i++)
            {
                if (playerList[i].CharacterPhotonView.IsMine)
                {
                    currentClientIndex = i;
                    break;
                }
            }

            int middleIndex = playerList.Count / 2;

            var temp = playerList[currentClientIndex];
            playerList.RemoveAt(currentClientIndex);
            playerList.Insert(middleIndex, temp);

            for (int i = 0; i < playerList.Count; i++)
            {
                if (i >= resultUIList.Count)
                    continue;

                CharacterControl player = playerList[i];
                SpawnPlayerPrefab(player.CharacterType, i);
                resultUIList[i].Init(player.KillCount, player.DealtDamage, player.DealtHeal, player.Nickname, player.CharacterPhotonView.ViewID);
            }

            if (isVictory)
            {
                victoryText.SetActive(true);
                defeatText.SetActive(false);
            }
            else
            {
                victoryText.SetActive(false);
                defeatText.SetActive(true);
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

            spawnedPrefab.GetComponentInChildren<WaitHabitTrigger>(true).PlayAnimation(isVictory);
        }

        public IEnumerator ShowResults()
        {
            CharacterControl.CurrentClientCharacter.SetState(SetCharState.ADD, CharState.INTERACTING_UI);
            IncreaseReturnCountText(0);

            blackTween.DORewind();
            blackTween.DOPlay();

            buttonTween1.DORewind();
            buttonTween1.DOPlay();

            buttonTween2.DORewind();
            buttonTween2.DOPlay();

            titleTween.DORewind();
            titleTween.DOPlay();

            SoundManager.Instance.StopBGM(1.0f);
            yield return new WaitForSeconds(0.5f);

            PlayBGM();
            StartCoroutine(VolumeAnimator.BlurBackground(depthOfField, 0.5f, 1f, 5f));

            for (int i = 0; i < resultUIList.Count; i++)
                resultUIList[i].gameObject.SetActive(false);

            for (int i = 0; i < resultUIList.Count; i++)
            {
                resultUIList[i].gameObject.SetActive(true);
                resultUIList[i].PlayTween();
                yield return new WaitForSeconds(SHOW_RESULT_INTERVAL);
            }
        }

        private void PlayBGM()
        {
            if (isVictory)
            {
                SoundManager.Instance.PlaySound(ConstStrings.SFX_VICTORY);
            }
            else
                SoundManager.Instance.PlayBGM(ConstStrings.BGM_DEFEAT);
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

        private void RequestReturnToLobby()
        {
            if (!PhotonNetwork.IsConnected)
                return;
            
            lobbyButton.onClick.RemoveListener(RequestReturnToLobby);
            ChangeLobbyButtonColor();

            int currentClientViewID = CharacterControl.CurrentClientCharacter.CharacterPhotonView.ViewID;

            photonView.RPC(nameof(SyncCheckbox), RpcTarget.All, currentClientViewID);
            photonView.RPC(nameof(TryReturnToLobby), RpcTarget.MasterClient, currentClientViewID);
        }

        private void ChangeLobbyButtonColor()
        {
            Image buttonImg = lobbyButton.GetComponent<Image>();
            buttonImg.color = readyColor;
        }

        [PunRPC]
        private void TryReturnToLobby(int viewID)
        {
            if (!PhotonNetwork.IsConnected)
                return;

            if (returnRequestList.Contains(viewID))
                return;
            
            returnRequestList.Add(viewID);

            photonView.RPC(nameof(SyncIncreaseReturnCountText), RpcTarget.All, returnRequestList.Count);
            photonView.RPC(nameof(SyncButtonSound), RpcTarget.All);

            if (returnRequestList.Count >= GameManager.Instance.PlayerDict.Count)
            {
                returnRequestList.Clear();
                GameManager.Instance.LevelController.SyncReturnToLobby();
            }
        }

        [PunRPC]
        private void SyncCheckbox(int viewID)
        {
            foreach (var resultUI in resultUIList)
            {
                if (resultUI.ViewID != viewID)
                    continue;

                resultUI.EnableReadyCheckbox();
                break;
            }
        }

        [PunRPC]
        private void SyncIncreaseReturnCountText(int currentCount)
        {
            IncreaseReturnCountText(currentCount);
        }

        [PunRPC]
        private void SyncButtonSound()
        {
            ButtonSound(returnRequestList.Count == GameManager.Instance.PlayerDict.Count);
        }

        private void IncreaseReturnCountText(int currentCount)
        {
            returnCountText.text = $"{currentCount}/{GameManager.Instance.PlayerDict.Count}";
        }
    }
}
