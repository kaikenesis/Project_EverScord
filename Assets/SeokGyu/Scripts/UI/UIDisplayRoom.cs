using System;
using TMPro;
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

            SetObjectsVisibility(false);

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

            SetObjectsVisibility(true);
        }

        private void HandleRoomLeft()
        {
            gameObject.SetActive(false);

            if (roomContainer != null)
            {
                roomContainer.SetActive(false);
            }

            SetObjectsVisibility(false);
        }

        private void HandleDisplayPlayers(List<string> players)
        {
            int i = 0;
            for (i = 0; i < players.Count; i++)
            {
                uiRoomPlayers[i].gameObject.SetActive(true);
                uiRoomPlayers[i].Initialize(players[i]);
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


        private void SetObjectsVisibility(bool bVisible)
        {
            int count = showObjects.Length;
            for (int i = 0; i < count; i++)
            {
                showObjects[i].SetActive(bVisible);
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
            if(inviteButton.activeSelf == true)
            {
                sendInvite.SetActive(false);
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
