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
        [SerializeField] private GameObject[] hideObjects;
        [SerializeField] private GameObject[] showObjects;
        [SerializeField] private Button[] masterButtons;
        private UIRoomPlayer[] uiRoomPlayers;

        public static Action OnLeaveRoom = delegate { };

        private void Awake()
        {
            PhotonRoomController.OnJoinRoom += HandleJoinRoom;
            PhotonRoomController.OnRoomLeft += HandleRoomLeft;
            PhotonRoomController.OnDisplayPlayers += HandleDisplayPlayers;

            Init();
        }
        private void OnDestroy()
        {
            PhotonRoomController.OnJoinRoom -= HandleJoinRoom;
            PhotonRoomController.OnRoomLeft -= HandleRoomLeft;
            PhotonRoomController.OnDisplayPlayers -= HandleDisplayPlayers;
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

            if (roomContainer != null) roomContainer.SetActive(true);

            SetObjectsVisibility(true);

            if (PhotonNetwork.IsMasterClient == false)
                SetActiveUI(false);
        }

        private void HandleRoomLeft()
        {
            gameObject.SetActive(false);

            if (roomContainer != null) roomContainer.SetActive(false);

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
                inviteButton.SetActive(true);
            }
            else
                inviteButton.SetActive(false);
        }
        #endregion


        private void SetObjectsVisibility(bool bVisible)
        {
            int count = showObjects.Length;
            for (int i = 0; i < count; i++)
            {
                showObjects[i].SetActive(bVisible);
            }

            count = hideObjects.Length;
            for (int i = 0; i < count; i++)
            {
                hideObjects[i].SetActive(!bVisible);
            }
        }

        private void SetActiveUI(bool bActive)
        {
            inviteButton.SetActive(false);

            for (int i = 0; i < masterButtons.Length; i++)
            {
                masterButtons[i].interactable = bActive;
            }
        }

        public void LeaveRoom()
        {
            OnLeaveRoom?.Invoke();
        }
    }
}
