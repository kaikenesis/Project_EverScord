using Photon.Chat;
using Photon.Realtime;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EverScord
{
    public class UIPlayer : MonoBehaviour
    {
        [SerializeField] private TMP_Text playerNameText;
        [SerializeField] private string playerName;
        [SerializeField] private Image onlineImage;
        [SerializeField] private GameObject inviteButton;

        #region Private Methods
        private void SetupUI()
        {
            Debug.Log(playerName);
            playerNameText.SetText(playerName);
            inviteButton.SetActive(false);
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
        }
        #endregion
    }
}