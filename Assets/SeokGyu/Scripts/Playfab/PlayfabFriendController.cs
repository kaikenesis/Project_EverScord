using System.Collections.Generic;
using PlayFab;
using PlayFab.MultiplayerModels;
using PlayFab.ClientModels;
using System;
using System.Linq;
using UnityEngine;
using EverScord;

public class PlayfabFriendController : MonoBehaviour
{
    public static Action<List<FriendInfo>> OnFriendListUpdated = delegate { };
    private List<FriendInfo> friends;

    private void Awake()
    {
        friends = new List<FriendInfo>();
        //PhotonConnector.
        UIAddFriends.OnAddFriend += HandleAddPlayfabFriend;
        UIFriend.OnRemoveFriend += HandleRemoveFriend;
    }

    private void OnDestroy()
    {
        UIAddFriends.OnAddFriend -= HandleAddPlayfabFriend;
        UIFriend.OnRemoveFriend -= HandleRemoveFriend;
    }

    private void Start()
    {

    }

    private void HandleAddPlayfabFriend(string name)
    {
        var request = new AddFriendRequest { FriendTitleDisplayName = name };
        PlayFabClientAPI.AddFriend(request, OnFriendAddedSuccess, OnFailure);
    }

    private void HandleRemoveFriend(string name)
    {
        string id = friends.FirstOrDefault(f => f.TitleDisplayName == name).ToString();
        var request = new RemoveFriendRequest { FriendPlayFabId = id };
        PlayFabClientAPI.RemoveFriend(request, OnFriendRemoveSuccess, OnFailure);
    }

    private void GetPlayfabFriends()
    {
        var request = new GetFriendsListRequest { XboxToken = null };
        PlayFabClientAPI.GetFriendsList(request, OnFriendsListSuccess, OnFailure);
    }

    private void OnFriendAddedSuccess(AddFriendResult result)
    {
        GetPlayfabFriends();
    }

    private void OnFriendsListSuccess(GetFriendsListResult result)
    {
        friends = result.Friends;
        OnFriendListUpdated?.Invoke(result.Friends);
    }

    private void OnFriendRemoveSuccess(RemoveFriendResult result)
    {
        GetPlayfabFriends();
    }

    private void OnFailure(PlayFabError error)
    {
        Debug.Log($"Playfab Friend Error occured: {error.GenerateErrorReport()}");
    }

    
}
