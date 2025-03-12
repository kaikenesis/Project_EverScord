using System;
using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine.UI;

namespace EverScord
{
    public class UIDisplayRoom : MonoBehaviour
    {
        [SerializeField] private UIRoomPlayer uiRoomPlayerPrefab;
        [SerializeField] private GameObject roomContainer;
        [SerializeField] private GameObject inviteButton;
        [SerializeField] private GameObject[] hideObjects;
        [SerializeField] private GameObject[] showObjects;
        [SerializeField] private Button[] singleOnlyButtons;
        [SerializeField] private Button[] masterOnlyButtons;
        [SerializeField] private Button[] gameSettingButtons;
        private UIRoomPlayer[] uiRoomPlayers;
        private List<string> playerNames = new List<string>();

        public static Action OnLeaveRoom = delegate { };
        public static Action OnVisibleObject = delegate { };

        private void Awake()
        {
            PhotonRoomController.OnJoinRoom += HandleJoinRoom;
            PhotonRoomController.OnRoomLeft += HandleRoomLeft;
            PhotonRoomController.OnDisplayPlayers += HandleDisplayPlayers;
            PhotonRoomController.OnUpdateRoom += HandleUpdateRoom;
            PhotonMatchController.OnUpdateUI += HandleUpdateUI;
            GameManager.OnUpdatePlayerData += HandleUpdatePortrait;

            Init();
        }

        private void Start()
        {
            if(!PhotonNetwork.InRoom)
            {
                OnHideObjects();

                gameObject.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            PhotonRoomController.OnJoinRoom -= HandleJoinRoom;
            PhotonRoomController.OnRoomLeft -= HandleRoomLeft;
            PhotonRoomController.OnDisplayPlayers -= HandleDisplayPlayers;
            PhotonRoomController.OnUpdateRoom -= HandleUpdateRoom;
            PhotonMatchController.OnUpdateUI -= HandleUpdateUI;
        }

        private void Init()
        {
            uiRoomPlayers = new UIRoomPlayer[3];
            for (int i = 0; i < 3; i++)
            {
                UIRoomPlayer uiRoomPlayer = Instantiate(uiRoomPlayerPrefab, roomContainer.transform);
                uiRoomPlayers[i] = uiRoomPlayer;
            }
            inviteButton.transform.SetAsLastSibling();
        }
        #region Handle Methods
        private void HandleJoinRoom()
        {
            gameObject.SetActive(true);

            if (roomContainer != null)
            {
                roomContainer.SetActive(true);
            }

            OnShowObjects();
        }

        private void HandleRoomLeft()
        {
            gameObject.SetActive(false);

            if (roomContainer != null)
            {
                roomContainer.SetActive(false);
            }

            OnHideObjects();
        }

        private void HandleDisplayPlayers(List<string> players)
        {
            playerNames = players;
            int i = 0;
            for (i = 0; i < players.Count; i++)
            {
                bool bMaster = false;
                if (PhotonNetwork.MasterClient.NickName == players[i])
                    bMaster = true;

                SyncPortrait(i, players[i], bMaster, 0, 0);
                
                uiRoomPlayers[i].gameObject.SetActive(true);
                
            }

            for (; i < 3; i++)
            {
                uiRoomPlayers[i].gameObject.SetActive(false);
            }

            if (players.Count < 3)
            {
                inviteButton.SetActive(PhotonNetwork.IsMasterClient);
            }
            else
            {
                inviteButton.SetActive(false);
            }
        }

        private void HandleUpdateRoom()
        {
            SetActiveMasterButton(PhotonNetwork.IsMasterClient);

            if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
            {
                SetActiveSingleButton(false);
            }
            else
            {
                SetActiveSingleButton(true);
            }
        }

        private void HandleUpdateUI(bool bActive)
        {
            SetActiveGameUI(bActive);
        }

        private void HandleUpdatePortrait(string nickName)
        {
            for (int i = 0; i < uiRoomPlayers.Length; i++)
            {
                if (uiRoomPlayers[i].gameObject.activeSelf == false)
                    break;
            }
        }

        [PunRPC]
        private void SyncPortrait(int playerNum, string name, bool bMaster, int characterNum, int jobNum)
        {
            uiRoomPlayers[playerNum].Initialize(name, bMaster, characterNum, jobNum);
        }
        #endregion // Handle Methods

        private void OnHideObjects()
        {
            for (int i = 0; i < hideObjects.Length; i++)
            {
                hideObjects[i].SetActive(false);
            }
        }

        private void OnShowObjects()
        {
            for (int i = 0; i < showObjects.Length; i++)
            {
                showObjects[i].SetActive(true);
            }

            OnVisibleObject?.Invoke();
        }

        private void SetActiveMasterButton(bool bActive)
        {
            for (int i = 0; i < masterOnlyButtons.Length; i++)
            {
                masterOnlyButtons[i].interactable = bActive;
            }
        }

        private void SetActiveSingleButton(bool bActive)
        {
            for (int i = 0; i < singleOnlyButtons.Length; i++)
            {
                singleOnlyButtons[i].interactable = bActive;
            }
        }

        private void SetActiveGameUI(bool bActive)
        {
            // 파티장만 초대버튼 활성화/비활성화
            // 나머지 버튼들은 비활성화
            if(inviteButton.activeSelf == false)
            {
                inviteButton.SetActive(PhotonNetwork.IsMasterClient);
            }
            else if(inviteButton.activeSelf == true)
            {
                inviteButton.SetActive(false);
            }

            for (int i = 0; i < gameSettingButtons.Length; i++)
            {
                gameSettingButtons[i].interactable = bActive;
            }
        }

        public void LeaveRoom()
        {
            OnLeaveRoom?.Invoke();
        }
    }
}
