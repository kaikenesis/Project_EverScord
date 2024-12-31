using UnityEngine;
using System;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun.UtilityScripts;
using EverScord;

public class PhotonRoomController : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameMode selectedGameMode;
    [SerializeField] private GameMode[] availableGameModes;
    private const string GAME_MODE = "GAMEMODE";

    public static Action<GameMode> OnJoinRoom = delegate { };
    public static Action<bool> OnRoomStatusChange = delegate { };
    public static Action OnRoomLeft = delegate { };
    public static Action<Player> OnOtherPlayerLeftRoom = delegate { };

    private void Awake()
    {
        UIGameMode.OnGameModeSelected += HandleGameModeSelected;
        UIInvite.OnRoomInviteAccept += HandleRoomInviteAccept;
        PhotonConnector.OnLobbyJoined += HandleLobbyJoined;
        UIDisplayRoom.OnLeaveRoom += HandleLeaveRoom;
        UIFriend.OnGetRoomStatus += HandleGetRoomStatus;
    }

    private void OnDestroy()
    {
        UIGameMode.OnGameModeSelected -= HandleGameModeSelected;
        UIInvite.OnRoomInviteAccept -= HandleRoomInviteAccept;
        PhotonConnector.OnLobbyJoined -= HandleLobbyJoined;
        UIDisplayRoom.OnLeaveRoom -= HandleLeaveRoom;
        UIFriend.OnGetRoomStatus -= HandleGetRoomStatus;
    }

    #region Handle Methods
    private void HandleGameModeSelected(GameMode gameMode)
    {
        if (!PhotonNetwork.IsConnectedAndReady) return;
        if (PhotonNetwork.InRoom) return;

        selectedGameMode = gameMode;
        Debug.Log($"Joining new {selectedGameMode.Name} game");
        JoinPhotonRoom();
    }

    private void HandleRoomInviteAccept(string roomName)
    {
        PlayerPrefs.SetString("PHOTONROOM", roomName);
        if(PhotonNetwork.InRoom)
        {
            OnRoomLeft?.Invoke();
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            if(PhotonNetwork.InLobby)
            {
                JoinPlayerRoom();
            }
        }
    }

    private void HandleLobbyJoined()
    {
        string roomName = PlayerPrefs.GetString("PHOTONROOM");
        if(!string.IsNullOrEmpty(roomName))
        {
            PhotonNetwork.JoinRoom(roomName);
            PlayerPrefs.SetString("PHOTONNAME", "");
        }
    }

    private void HandleLeaveRoom()
    {
        if(PhotonNetwork.InRoom)
        {
            OnRoomLeft?.Invoke();
            PhotonNetwork.LeaveRoom();
        }
    }

    private void HandleGetRoomStatus()
    {
        OnRoomStatusChange?.Invoke(PhotonNetwork.InRoom);
    }
    #endregion

    #region Private Methods
    private void JoinPhotonRoom()
    {
        Hashtable expectedCustomRoomProperties = new Hashtable() { { GAME_MODE, selectedGameMode.Name } };

        PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
    }

    private void CreatePhotonRoom()
    {
        string roomName = Guid.NewGuid().ToString();
        RoomOptions roomOptions = GetRoomOptions();

        PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
    }

    private RoomOptions GetRoomOptions()
    {
        RoomOptions ro = new RoomOptions();
        ro.IsOpen = true;
        ro.IsVisible = true;
        ro.MaxPlayers = selectedGameMode.MaxPlayers;

        string[] roomProperties = { GAME_MODE };

        Hashtable customRoomProperties = new Hashtable() { { GAME_MODE, selectedGameMode.Name } };

        ro.CustomRoomPropertiesForLobby = roomProperties;
        ro.CustomRoomProperties = customRoomProperties;

        return ro;
    }
    private void DebugPlayerList()
    {
        string players = "";
        Dictionary<int, Player> playerList = PhotonNetwork.CurrentRoom.Players;
        for (int i = 0; i < playerList.Count; i++)
        {
            players += $"{playerList[i].NickName}, ";
        }
        Debug.Log($"Current Room Players: {players}");
    }
    private GameMode GetRoomGameMode()
    {
        string gameModeName = (string)PhotonNetwork.CurrentRoom.CustomProperties[GAME_MODE];
        GameMode gameMode = null;
        for (int i = 0; i < availableGameModes.Length; i++)
        {
            if (string.Compare(availableGameModes[i].Name, gameModeName) == 0)
            {
                gameMode = availableGameModes[i];
                break;
            }
        }
        return gameMode;
    }
    #endregion

    #region Photon Callbacks
    public override void OnCreatedRoom()
    {
        Debug.Log($"Created Room Successful.\nPhoton RoomName : {PhotonNetwork.CurrentRoom.Name}");
    }
    public override void OnJoinedRoom()
    {
        Debug.Log($"Joined Room Successful.\nPhoton RoomName : {PhotonNetwork.CurrentRoom.Name}");
        DebugPlayerList();

        selectedGameMode = GetRoomGameMode();
        OnJoinRoom?.Invoke(selectedGameMode);
        OnRoomStatusChange?.Invoke(PhotonNetwork.InRoom);
    }
    public override void OnLeftRoom()
    {
        Debug.Log("You have left Photon Room");
        selectedGameMode = null;
        OnRoomStatusChange?.Invoke(PhotonNetwork.InRoom);
    }
    // RandomRoom Join 실패 시 오류 콜백 함수 실행
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"Join Random Failed {returnCode} : {message}");
        CreatePhotonRoom();
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log($"Join Room Failed {returnCode} : {message}");
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"Another player has joined the room : {newPlayer.NickName}");
        DebugPlayerList();
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"Player has left the room : {otherPlayer.NickName}");
        OnOtherPlayerLeftRoom?.Invoke(otherPlayer);
        DebugPlayerList();
    }
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
    }

    #endregion
}
