using System;
using TMPro;
using UnityEngine;

namespace EverScord
{
    public class UIInvite : MonoBehaviour
    {
        [SerializeField] private string friendName;
        [SerializeField] private string roomName;
        [SerializeField] private TMP_Text friendNameText;

        public static Action<UIInvite> OnInviteAccept = delegate { };
        public static Action<string> OnRoomInviteAccept = delegate { };
        public static Action<UIInvite> OnInviteDecline = delegate { };

        public void Initialize(string friendName, string roomName)
        {
            this.friendName = friendName;
            this.roomName = roomName;

            friendNameText.SetText($"{friendName} 님이 파티에 초대했습니다.");
        }

        public void AcceptInvite()
        {
            OnInviteAccept?.Invoke(this);
            if (!string.IsNullOrEmpty(roomName))
            {
                OnRoomInviteAccept?.Invoke(roomName);
            }
        }

        public void DeclineInvite()
        {
            OnInviteDecline?.Invoke(this);
        }
    }
}
