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
        [SerializeField] private GameObject exitButton;
        [SerializeField] private GameObject roomContainer;
        [SerializeField] private GameObject sendInviteContainer;
        [SerializeField] private GameObject inviteButton;
        [SerializeField] private GameObject[] hideObjects;
        [SerializeField] private GameObject[] showObjects;
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

            Instantiate(inviteButton, roomContainer.transform);
        }

        private void HandleJoinRoom()
        {
            gameObject.SetActive(true);

            if(exitButton != null) exitButton.SetActive(true);
            if(roomContainer != null) roomContainer.SetActive(true);

            SetObjectsVisibility(true);
        }

        private void HandleRoomLeft()
        {
            gameObject.SetActive(false);

            if (exitButton != null) exitButton.SetActive(false);
            if (roomContainer != null) roomContainer.SetActive(false);

            SetObjectsVisibility(false);
        }

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

        //Room내 플레이어 목록 UI 갱신(수정 필요)
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

        public void LeaveRoom()
        {
            OnLeaveRoom?.Invoke();
        }

        public void ToggleSendInviteContainor()
        {
            Debug.Log("Click");
            sendInviteContainer.SetActive(!sendInviteContainer.activeSelf);
        }
    }
}
