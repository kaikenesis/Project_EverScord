using Photon.Pun;
using Photon.Realtime;
using PlayFabFriendInfo = PlayFab.ClientModels.FriendInfo;
using PhotonFriendInfo = Photon.Realtime.FriendInfo;
using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PhotonFriendController : MonoBehaviourPunCallbacks
{
    [SerializeField] private float refreshCooldown;
    [SerializeField] private float refreshCountdown;
    [SerializeField] private List<PlayFabFriendInfo> friendList;

    public static Action<List<PhotonFriendInfo>> OnDisplayFriends = delegate { };
    private void Awake()
    {
        friendList = new List<PlayFabFriendInfo>();
        PlayfabFriendController.OnFriendListUpdated += HandleFriendsUpdated;
    }

    private void OnDestroy()
    {
        PlayfabFriendController.OnFriendListUpdated -= HandleFriendsUpdated;
    }

    private void Update()
    {
        RefreshFriends();
    }

    private void RefreshFriends()
    {
        if (PhotonNetwork.InRoom) return;

        if (refreshCountdown > 0)
        {
            refreshCountdown -= Time.deltaTime;
        }
        else
        {
            refreshCountdown = refreshCooldown;
            if (PhotonNetwork.InRoom) return;
            FindPhotonFriends(friendList);
        }
    }
    private void HandleFriendsUpdated(List<PlayFabFriendInfo> friends)
    {
        friendList = friends;
        FindPhotonFriends(friends);
    }

    private void FindPhotonFriends(List<PlayFabFriendInfo> friends)
    {
        Debug.Log($"Handle getting Photon friends {friends.Count}");
        if (friends.Count != 0)
        {
            string[] friendDisplayNames = friends.Select(f => f.TitleDisplayName).ToArray();
            PhotonNetwork.FindFriends(friendDisplayNames);
        }
        else
        {
            List<PhotonFriendInfo> friendList = new List<PhotonFriendInfo>();
            OnDisplayFriends?.Invoke(friendList);
        }
    }

    public override void OnFriendListUpdate(List<PhotonFriendInfo> friendList)
    {
        OnDisplayFriends?.Invoke(friendList);
    }
}
