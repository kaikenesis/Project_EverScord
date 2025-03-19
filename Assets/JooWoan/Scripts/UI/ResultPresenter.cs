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
        [SerializeField] private DOTweenAnimation buttonTween, titleTween, blackTween;
        [SerializeField] private Button lobbyButton;
        private DepthOfField depthOfField;
        private bool isVictory;

        void Awake()
        {
            GameManager.Instance.InitControl(this);

            uiHub.SetActive(false);
            blackTween.gameObject.SetActive(true);

            readyPlayerCount = 0;
            isVictory = false;

            lobbyButton.onClick.AddListener(LevelControl.ReturnToLobby);
        }

        void Start()
        {
            InitDepthOfField();
        }

        void OnDisable()
        {
            lobbyButton.onClick.RemoveListener(LevelControl.ReturnToLobby);

            if (depthOfField)
            {
                depthOfField.focusDistance.value = 1f;
                depthOfField.active = false;
            }
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
        }

        private void SetupUI()
        {
            for (int i = 0; i < ingameUIList.Count; i++)
                ingameUIList[i].SetActive(false);

            GameManager.Instance.GameOverController.EnableUI(false);
            
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
                resultUIList[i].Init(player.KillCount, player.DealtDamage, player.DealtHeal, player.Nickname);
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
            blackTween.DORewind();
            blackTween.DOPlay();

            buttonTween.DORewind();
            buttonTween.DOPlay();

            titleTween.DORewind();
            titleTween.DOPlay();

            yield return new WaitForSeconds(0.5f);

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
    }
}
