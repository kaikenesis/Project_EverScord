using System;
using TMPro;
using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

namespace EverScord
{
    public class UIDisplayRoom : MonoBehaviour
    {
        [SerializeField] private TMP_Text roomGameModeText;
        [SerializeField] private GameObject exitButton;
        [SerializeField] private GameObject roomContainer;
        [SerializeField] private GameObject[] hideObjects;
        [SerializeField] private GameObject[] showObjects;

        public static Action OnLeaveRoom = delegate { };

        private void Awake()
        {
            PhotonRoomController.OnJoinRoom += HandleJoinRoom;
            PhotonRoomController.OnRoomLeft += HandleRoomLeft;
            
            gameObject.SetActive(false);
        }
        private void OnDestroy()
        {
            PhotonRoomController.OnJoinRoom -= HandleJoinRoom;
            PhotonRoomController.OnRoomLeft -= HandleRoomLeft;
        }

        private void HandleJoinRoom(GameMode gameMode)
        {
            gameObject.SetActive(true);
            //roomGameModeText.SetText(PhotonNetwork.CurrentRoom.CustomProperties["GAMEMODE"].ToString());

            if(exitButton != null) exitButton.SetActive(true);
            if(roomContainer != null) roomContainer.SetActive(true);

            int count = hideObjects.Length;
            for (int i = 0; i < count; i++)
            {
                hideObjects[i].SetActive(false);
            }
        }

        private void HandleRoomLeft()
        {
            gameObject.SetActive(false);
            //roomGameModeText.SetText("JOINING ROOM");

            if (exitButton != null) exitButton.SetActive(false);
            if (roomContainer != null) roomContainer.SetActive(false);

            int count = showObjects.Length;
            for (int i = 0; i < count; i++)
            {
                showObjects[i].SetActive(true);
            }
        }

        // Room내 플레이어 목록 UI 갱신 (수정 필요)
        //private void HandleDisplayChatFriends(List<string> friends)
        //{
        //    foreach (Transform child in friendContainer)
        //    {
        //        Destroy(child.gameObject);
        //    }

        //    foreach (string friend in friends)
        //    {
        //        UIFriend uifriend = Instantiate(uiFriendPrefab, friendContainer);
        //        uifriend.Initialize(friend);
        //        contentRect.sizeDelta += increaseSize;
        //    }
        //}

        public void LeaveRoom()
        {
            OnLeaveRoom?.Invoke();
        }
    }
}
