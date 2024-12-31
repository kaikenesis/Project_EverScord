using System;
using TMPro;
using UnityEngine;
using Photon.Pun;

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
        }
        private void OnDestroy()
        {
            PhotonRoomController.OnJoinRoom -= HandleJoinRoom;
            PhotonRoomController.OnRoomLeft -= HandleRoomLeft;
        }

        private void HandleJoinRoom(GameMode gameMode)
        {
            roomGameModeText.SetText(PhotonNetwork.CurrentRoom.CustomProperties["GAMEMODE"].ToString());

            exitButton.SetActive(true);
            roomContainer.SetActive(true);

            int count = hideObjects.Length;
            for (int i = 0; i < count; i++)
            {
                hideObjects[i].SetActive(false);
            }
        }

        private void HandleRoomLeft()
        {
            roomGameModeText.SetText("JOINING ROOM");

            exitButton.SetActive(false);
            roomContainer.SetActive(false);

            int count = showObjects.Length;
            for (int i = 0; i < count; i++)
            {
                showObjects[i].SetActive(true);
            }
        }

        public void LeaveRoom()
        {
            OnLeaveRoom?.Invoke();
        }
    }
}
