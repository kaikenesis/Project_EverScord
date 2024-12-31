using System;
using TMPro;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using Photon.Chat;

public class UIFriend : MonoBehaviour
{
    [SerializeField] private TMP_Text friendNameText;
    [SerializeField] private string friendName;
    [SerializeField] private Image onlineImage;
    [SerializeField] private Color onlineColor;
    [SerializeField] private Color offlineColor;

    public static Action<string> OnRemoveFriend = delegate { };
    public static Action<string> OnInviteFriend = delegate { };
    public static Action<string> OnGetCurrentStatus = delegate { };

    #region Private Methods

    private void Awake()
    {
        PhotonChatController.OnStatusUpdated += HandleStatusUpdated;
        PhotonChatFriendController.OnStatusUpdated += HandleStatusUpdated;
    }

    private void OnDestroy()
    {
        PhotonChatController.OnStatusUpdated -= HandleStatusUpdated;
        PhotonChatFriendController.OnStatusUpdated -= HandleStatusUpdated;
    }

    private void OnEnable()
    {
        if (string.IsNullOrEmpty(friendName)) return;
        OnGetCurrentStatus?.Invoke(friendName);
    }

    private void HandleStatusUpdated(PhotonStatus status)
    {
        if(string.Compare(friendName, status.playerName) == 0)
        {
            Debug.Log($"Updating status in UI for {status.playerName} to status {status.status}");
            SetStatus(status.status);
        }
        else
        {
            Debug.Log($"Good for nothing HandleStatusUpdated {status.playerName}");
        }
    }

    private void SetupUI()
    {
        Debug.Log(friendName);
        friendNameText.SetText(friendName);
    }

    private void SetStatus(int status)
    {
        if (status == ChatUserStatus.Online)
        {
            onlineImage.color = onlineColor;
        }
        else
        {
            onlineImage.color = offlineColor;
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
        this.friendName = friendName;

        SetupUI();
        OnGetCurrentStatus?.Invoke(friendName);
    }

    public void RemoveFriend()
    {
        Debug.Log($"Clicked to remove friend {friendName}");
        OnRemoveFriend?.Invoke(friendName);
    }

    public void InviteFriend()
    {
        Debug.Log($"Clicked to invite friend {friendName}");
        OnInviteFriend?.Invoke(friendName);
    }

    #endregion
}
