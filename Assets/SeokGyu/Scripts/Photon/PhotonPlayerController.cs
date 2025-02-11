using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PhotonPlayerController : MonoBehaviour
{
    [SerializeField] private float refreshCooldown;
    [SerializeField] private float refreshCountdown;
    [SerializeField] private List<FriendInfo> playerList = new List<FriendInfo>();

    public static Action<List<FriendInfo>> OnDisplayPlayers = delegate { };

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
            FindPhotonFriends(playerList);
        }
    }
    private void HandleFriendsUpdated(List<FriendInfo> friends)
    {
        playerList = friends;
        FindPhotonFriends(friends);
    }

    private void FindPhotonFriends(List<FriendInfo> friends)
    {
        Debug.Log($"Handle getting Photon friends {friends.Count}");
        if (friends.Count != 0)
        {
            string[] friendDisplayNames = friends.Select(f => f.UserId).ToArray();
            PhotonNetwork.FindFriends(friendDisplayNames);
        }
        else
        {
            List<FriendInfo> friendList = new List<FriendInfo>();
            OnDisplayPlayers?.Invoke(friendList);
        }
    }
}
