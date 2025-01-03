using System;
using TMPro;
using UnityEngine;

public class UIInvite : MonoBehaviour
{
    [SerializeField] private string friendName;
    [SerializeField] private string roomName;
    [SerializeField] private TMP_Text friendNameText;

    public static Action<UIInvite> OnInviteAccept = delegate { };
    public static Action<string> OnRoomInviteAccept = delegate { };
    public static Action<UIInvite> OnInviteDecline = delegate { };
    public static Action<string> OnPartyInviteAccept = delegate { };

    public void Initialize(string friendName, string roomName)
    {
        this.friendName = friendName;
        this.roomName = roomName;

        friendNameText.SetText($"Invite from\n{friendName}");
    }

    public void AcceptInvite()
    {
        OnInviteAccept?.Invoke(this);
        if(string.IsNullOrEmpty(roomName))
        {
            OnPartyInviteAccept?.Invoke(friendName);
        }
        else
        {
            OnRoomInviteAccept?.Invoke(roomName);
        }
    }

    public void DeclineInvite()
    {
        OnInviteDecline?.Invoke(this);
    }
}
