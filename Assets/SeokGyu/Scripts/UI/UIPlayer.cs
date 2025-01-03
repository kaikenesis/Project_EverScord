using Photon.Chat;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EverScord
{
    public class UIPlayer : MonoBehaviour
    {
        [SerializeField] private TMP_Text playerNameText;
        [SerializeField] private string playerName;
        [SerializeField] private bool isOnline;
        [SerializeField] private Image onlineImage;
        [SerializeField] private GameObject inviteButton;
        [SerializeField] private Color onlineColor;
        [SerializeField] private Color offlineColor;

        public static Action<string> OnRemoveFriend = delegate { };
        public static Action<string> OnInviteFriend = delegate { };
        public static Action<string> OnGetCurrentStatus = delegate { };
        public static Action OnGetRoomStatus = delegate { };

        #region Private Methods

        private void Awake()
        {
            PhotonChatController.OnStatusUpdated += HandleStatusUpdated;
            PhotonChatFriendController.OnStatusUpdated += HandleStatusUpdated;
            PhotonRoomController.OnRoomStatusChange += HandleInRoom;
        }

        private void OnDestroy()
        {
            PhotonChatController.OnStatusUpdated -= HandleStatusUpdated;
            PhotonChatFriendController.OnStatusUpdated -= HandleStatusUpdated;
            PhotonRoomController.OnRoomStatusChange -= HandleInRoom;
        }

        private void OnEnable()
        {
            if (string.IsNullOrEmpty(playerName)) return;
            OnGetCurrentStatus?.Invoke(playerName);
            OnGetRoomStatus?.Invoke();
        }

        private void HandleStatusUpdated(PhotonStatus status)
        {
            if (string.Compare(playerName, status.playerName) == 0)
            {
                Debug.Log($"Updating status in UI for {status.playerName} to status {status.status}");
                SetStatus(status.status);
            }
            else
            {
                Debug.Log($"Good for nothing HandleStatusUpdated {status.playerName}");
            }
        }

        private void HandleInRoom(bool inRoom)
        {
            Debug.Log($"Updating invite ui to {inRoom}");
            inviteButton.SetActive(inRoom && isOnline);
        }

        private void SetupUI()
        {
            Debug.Log(playerName);
            playerNameText.SetText(playerName);
            inviteButton.SetActive(false);
        }

        private void SetStatus(int status)
        {
            if (status == ChatUserStatus.Online)
            {
                if (onlineImage != null) onlineImage.color = onlineColor;
                isOnline = true;
                OnGetRoomStatus?.Invoke();
            }
            else
            {
                if(onlineImage != null) onlineImage.color = offlineColor;
                isOnline = false;
                inviteButton.SetActive(false);
            }
        }

        #endregion

        #region Public Methods

        public void Initialize(FriendInfo friend)
        {
            Debug.Log($"{friend.UserId} is online: {friend.IsOnline} ; in room: {friend.IsInRoom} ; room name: {friend.Room}");
            Initialize(friend.UserId);
        }

        public void Initialize(string friendName)
        {
            Debug.Log($"{friendName} is added");
            this.playerName = friendName;

            SetupUI();
            OnGetCurrentStatus?.Invoke(friendName);
        }

        public void InvitePlayer()
        {
            Debug.Log($"Clicked to invite friend {playerName}");
            OnInviteFriend?.Invoke(playerName);
        }

        #endregion
    }
}