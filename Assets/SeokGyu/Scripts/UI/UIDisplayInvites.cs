using System.Collections.Generic;
using UnityEngine;

namespace EverScord
{
    public class UIDisplayInvites : MonoBehaviour
    {
        [SerializeField] private Transform inviteContainer;
        [SerializeField] private UIInvite uiInvitePrefab;
        [SerializeField] private Vector2 originalSize;
        [SerializeField] private Vector2 increaseSize;

        private RectTransform contentRect;
        private List<UIInvite> invites;

        private void Awake()
        {
            invites = new List<UIInvite>();
            contentRect = inviteContainer.GetComponent<RectTransform>();
            originalSize = contentRect.sizeDelta;
            increaseSize = new Vector2(0, uiInvitePrefab.GetComponent<RectTransform>().sizeDelta.y);

            PhotonChatController.OnRoomInvite += HandleRoomInvite;
            UIInvite.OnInviteAccept += HandleInviteAccept;
            UIInvite.OnInviteDecline += HandleInviteDecline;
        }

        private void OnDestroy()
        {
            PhotonChatController.OnRoomInvite -= HandleRoomInvite;
            UIInvite.OnInviteAccept -= HandleInviteAccept;
            UIInvite.OnInviteDecline -= HandleInviteDecline;
        }

        private void HandleRoomInvite(string friend, string room)
        {
            if (GameManager.Instance.userData.curPhotonState != EPhotonState.NONE) return;

            Debug.Log($"Room invite for {friend} to room {room}");
            UIInvite uiInvite = Instantiate(uiInvitePrefab, inviteContainer);
            uiInvite.Initialize(friend, room);
            contentRect.sizeDelta += increaseSize;
            invites.Add(uiInvite);
        }

        private void HandleInviteAccept(UIInvite invite)
        {
            if (invites.Contains(invite))
            {
                invites.Remove(invite);
                Destroy(invite.gameObject);
            }
        }

        private void HandleInviteDecline(UIInvite invite)
        {
            if (invites.Contains(invite))
            {
                invites.Remove(invite);
                Destroy(invite.gameObject);
            }
        }
    }
}