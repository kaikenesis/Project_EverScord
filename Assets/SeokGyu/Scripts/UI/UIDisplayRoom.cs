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
        [SerializeField] private GameObject sendInvite;
        [SerializeField] private GameObject[] hideObjects;
        [SerializeField] private GameObject[] showObjects;
        [SerializeField] private Button[] singleOnlyButtons;
        [SerializeField] private Button[] masterOnlyButtons;
        [SerializeField] private Button[] gameSettingButtons;
        private UIRoomPlayer[] uiRoomPlayers;

        public static Action OnLeaveRoom = delegate { };

        private void Awake()
        {
            PhotonRoomController.OnJoinRoom += HandleJoinRoom;
            PhotonRoomController.OnRoomLeft += HandleRoomLeft;
            PhotonRoomController.OnDisplayPlayers += HandleDisplayPlayers;
            PhotonRoomController.OnUpdateRoom += HandleUpdateRoom;
            PhotonMatchController.OnUpdateUI += HandleUpdateUI;

            Init();
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
            gameObject.SetActive(false);

            OnHideObjects();

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
            int i = 0;
            for (i = 0; i < players.Count; i++)
            {
                bool bMaster = false;
                uiRoomPlayers[i].gameObject.SetActive(true);
                if (PhotonNetwork.MasterClient.NickName == players[i])
                    bMaster = true;
                uiRoomPlayers[i].Initialize(players[i], bMaster);
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
                if (bActive == false)
                {
                    InitToggleButtons(inviteButton);
                }
            }

            for (int i = 0; i < gameSettingButtons.Length; i++)
            {
                gameSettingButtons[i].interactable = bActive;
                if(bActive == false)
                {
                    InitToggleButtons(gameSettingButtons[i].gameObject);
                }
            }
        }

        private void InitToggleButtons(GameObject gameObject)
        {
            UIToggleButton uiToggle = gameObject.gameObject.GetComponent<UIToggleButton>();
            if (uiToggle != null)
            {
                uiToggle.Init();
            }
        }

        public void LeaveRoom()
        {
            OnLeaveRoom?.Invoke();
        }
    }
}
