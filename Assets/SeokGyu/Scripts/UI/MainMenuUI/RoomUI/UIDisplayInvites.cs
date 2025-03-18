using System.Collections.Generic;
using UnityEngine;

namespace EverScord
{
    public class UIDisplayInvites : MonoBehaviour
    {
        [SerializeField] private Transform inviteContainer;
        [SerializeField] private UIReceiveInvite uiInvitePrefab;
        [SerializeField] private Vector2 originalSize;
        [SerializeField] private Vector2 increaseSize;

        private RectTransform contentRect;
        private List<UIReceiveInvite> invites;

        private void Awake()
        {
            invites = new List<UIReceiveInvite>();
            contentRect = inviteContainer.GetComponent<RectTransform>();
            originalSize = contentRect.sizeDelta;
            increaseSize = new Vector2(0, uiInvitePrefab.GetComponent<RectTransform>().sizeDelta.y);

            PhotonChatController.OnRoomInvite += HandleRoomInvite;
            UIReceiveInvite.OnInviteAccept += HandleInviteAccept;
            UIReceiveInvite.OnInviteDecline += HandleInviteDecline;
        }

        private void OnDestroy()
        {
            PhotonChatController.OnRoomInvite -= HandleRoomInvite;
            UIReceiveInvite.OnInviteAccept -= HandleInviteAccept;
            UIReceiveInvite.OnInviteDecline -= HandleInviteDecline;
        }

        private void HandleRoomInvite(string friend, string room)
        {
            if (GameManager.Instance.PhotonData.state != PhotonData.EState.NONE) return;

            Debug.Log($"Room invite for {friend} to room {room}");
            UIReceiveInvite uiInvite = Instantiate(uiInvitePrefab, inviteContainer);
            uiInvite.Initialize(friend, room);
            contentRect.sizeDelta += increaseSize;
            invites.Add(uiInvite);
        }

        private void HandleInviteAccept(UIReceiveInvite invite)
        {
            if (invites.Contains(invite))
            {
                invites.Remove(invite);
                Destroy(invite.gameObject);
            }
        }

        private void HandleInviteDecline(UIReceiveInvite invite)
        {
            if (invites.Contains(invite))
            {
                invites.Remove(invite);
                Destroy(invite.gameObject);
            }
        }
    }
}