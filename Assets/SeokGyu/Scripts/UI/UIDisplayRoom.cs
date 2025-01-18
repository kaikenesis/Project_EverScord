using System;
using TMPro;
using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

namespace EverScord
{
    public class UIDisplayRoom : MonoBehaviour
    {
        [SerializeField] private UIRoomPlayer uiRoomPlayerPrefab;
        [SerializeField] private GameObject exitButton;
        [SerializeField] private GameObject roomContainer;
        [SerializeField] private GameObject inviteButton;
        [SerializeField] private GameObject[] hideObjects;
        [SerializeField] private GameObject[] showObjects;

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
            foreach (Transform child in roomContainer.transform)
            {
                Destroy(child.gameObject);
            }

            foreach (string player in players)
            {
                UIRoomPlayer uiRoomPlayer = Instantiate(uiRoomPlayerPrefab, roomContainer.transform);
                uiRoomPlayer.Initialize(player);
            }

            if(players.Count < 3)
            {
                Instantiate(inviteButton, roomContainer.transform);
            }
        }

        public void LeaveRoom()
        {
            OnLeaveRoom?.Invoke();
        }
    }
}
