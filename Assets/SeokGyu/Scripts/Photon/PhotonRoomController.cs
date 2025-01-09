using UnityEngine;
using System;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using ExitGames.Client.Photon;

namespace EverScord
{
    public class PhotonRoomController : MonoBehaviourPunCallbacks
    {
        [SerializeField] private GameMode selectedGameMode;
        [SerializeField] private GameMode[] availableGameModes;
        //private const string GAME_MODE = "GAMEMODE";
        private bool bMatch = false;

        public static Action<GameMode> OnJoinRoom = delegate { };
        public static Action<bool> OnRoomStatusChange = delegate { };
        public static Action OnRoomLeft = delegate { };
        public static Action<Player> OnOtherPlayerLeftRoom = delegate { };
        public static Action<List<string>> OnDisplayPlayers = delegate { };
        public static Action OnMatchSoloPlay = delegate { };
        public static Action<string, string> OnMatchMultiPlay = delegate { };
        public static Action OnJoinedMatch = delegate { };

        private void Awake()
        {
            //UIGameMode.OnGameModeSelected += HandleGameModeSelected;
            UIInvite.OnRoomInviteAccept += HandleRoomInviteAccept;
            PhotonConnector.OnLobbyJoined += HandleLobbyJoined;
            UIDisplayRoom.OnLeaveRoom += HandleLeaveRoom;
            UIFriend.OnGetRoomStatus += HandleGetRoomStatus;

            PlayerPrefs.SetString("PHOTONROOM", "");
        }

        private void OnDestroy()
        {
            //UIGameMode.OnGameModeSelected -= HandleGameModeSelected;
            UIInvite.OnRoomInviteAccept -= HandleRoomInviteAccept;
            PhotonConnector.OnLobbyJoined -= HandleLobbyJoined;
            UIDisplayRoom.OnLeaveRoom -= HandleLeaveRoom;
            UIFriend.OnGetRoomStatus -= HandleGetRoomStatus;
        }

        #region Handle Methods
        //private void HandleGameModeSelected(GameMode gameMode)
        //{
        //    if (!PhotonNetwork.IsConnectedAndReady) return;
        //    if (PhotonNetwork.InRoom) return;

        //    selectedGameMode = gameMode;
        //    Debug.Log($"Joining new {selectedGameMode.Name} game");
        //    JoinPhotonRoom();
        //}

        private void HandleRoomInviteAccept(string roomName)
        {
            PlayerPrefs.SetString("PHOTONROOM", roomName);
            if (PhotonNetwork.InRoom)
            {
                OnRoomLeft?.Invoke();
                PhotonNetwork.LeaveRoom();
            }
            else
            {
                if (PhotonNetwork.InLobby)
                {
                    PhotonNetwork.JoinRoom(roomName);
                    PlayerPrefs.SetString("PHOTONROOM", "");
                }
            }
        }

        private void HandleLobbyJoined()
        {
            string roomName = PlayerPrefs.GetString("PHOTONROOM");
            if (!string.IsNullOrEmpty(roomName))
            {
                PhotonNetwork.JoinRoom(roomName);
                PlayerPrefs.SetString("PHOTONNAME", "");
            }
            else
            {
                CreatePhotonRoom();
            }
        }

        private void HandleLeaveRoom()
        {
            if (PhotonNetwork.InRoom)
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
        //private void JoinPhotonRoom()
        //{
        //    Hashtable expectedCustomRoomProperties = new Hashtable() { { GAME_MODE, selectedGameMode.Name } };

        //    PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 0);
        //}

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
            ro.IsVisible = false;
            ro.PublishUserId = true;
            ro.MaxPlayers = selectedGameMode.MaxPlayers;

            return ro;
        }
        private void DebugPlayerList()
        {
            string players = "";
            Dictionary<int, Player> playerList = PhotonNetwork.CurrentRoom.Players;
            int key = 0;
            for (int i = 1; i <= playerList.Count; key++)
            {
                if (playerList.ContainsKey(key) == true)
                {
                    players += $"{playerList[key].NickName}, ";
                    i++;
                }
            }
            Debug.Log($"Current Room Players: {players}");
        }
        //private GameMode GetRoomGameMode()
        //{
        //    string gameModeName = (string)PhotonNetwork.CurrentRoom.CustomProperties[GAME_MODE];
        //    GameMode gameMode = null;
        //    for (int i = 0; i < availableGameModes.Length; i++)
        //    {
        //        if (string.Compare(availableGameModes[i].Name, gameModeName) == 0)
        //        {
        //            gameMode = availableGameModes[i];
        //            break;
        //        }
        //    }
        //    return gameMode;
        //}
        private void DisplayRoomPlayers()
        {
            List<string> players = new List<string>();
            Dictionary<int, Player> playerList = PhotonNetwork.CurrentRoom.Players;
            int key = 0;
            for (int i = 1; i <= playerList.Count; key++)
            {
                if (playerList.ContainsKey(key) == true)
                {
                    players.Add(playerList[key].NickName);
                    i++;
                }
            }

            OnDisplayPlayers?.Invoke(players);
        }
        #endregion

        #region Public Methods
        public void OnClickedSoloPlay()
        {
            bMatch = true;
            OnMatchSoloPlay?.Invoke();
        }

        public void OnClickedMultiPlay()
        {
            bMatch = true;
            // 필요한 직업과 난이도 레벨
            //OnMatchMultiPlay?.Invoke();
        }
        #endregion

        #region Photon Callbacks
        public override void OnCreatedRoom()
        {
            Debug.Log($"Created Room Successful.\nPhoton RoomName : {PhotonNetwork.CurrentRoom.Name}");
            if(bMatch)
            {

            }
            else
            {

            }
        }
        public override void OnJoinedRoom()
        {
            Debug.Log($"Joined Room Successful.\nPhoton RoomName : {PhotonNetwork.CurrentRoom.Name}");
            if (bMatch)
            {
                // 현재 입장한 방에서 매칭조건이 부합하는지 확인. 부적합하다면 LeaveRoom()
                OnJoinedMatch?.Invoke();
            }
            else
            {
                DebugPlayerList();

                //selectedGameMode = GetRoomGameMode();
                OnJoinRoom?.Invoke(selectedGameMode);
                OnRoomStatusChange?.Invoke(PhotonNetwork.InRoom);
                DisplayRoomPlayers();
            }
        }
        public override void OnLeftRoom()
        {
            Debug.Log("You have left Photon Room");
            if (bMatch)
            {
            }
            else
            {
                selectedGameMode = null;
                OnRoomStatusChange?.Invoke(PhotonNetwork.InRoom);
            }
        }
        // RandomRoom Join 실패 시 오류 콜백 함수 실행
        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log($"Join Random Failed {returnCode} : {message}");
            if (bMatch)
            {

            }
            else
            {
                CreatePhotonRoom();
            }
        }
        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.Log($"Join Room Failed {returnCode} : {message}");
            if (bMatch)
            {

            }
            else
            {
                CreatePhotonRoom();
            }
        }
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            Debug.Log($"Another player has joined the room : {newPlayer.NickName}");
            if (bMatch)
            {

            }
            else
            {
                DebugPlayerList();
                DisplayRoomPlayers();
            }
        }
        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            Debug.Log($"Player has left the room : {otherPlayer.NickName}");
            if (bMatch)
            {

            }
            else
            {
                OnOtherPlayerLeftRoom?.Invoke(otherPlayer);
                DebugPlayerList();
                DisplayRoomPlayers();
            }
        }
        public override void OnMasterClientSwitched(Player newMasterClient)
        {
        }

        #endregion
    }
}