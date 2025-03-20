using UnityEngine;
using System;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using WebSocketSharp;
using static EverScord.PlayerData;
using EverScord.UI;

namespace EverScord
{
    public class PhotonRoomController : MonoBehaviourPunCallbacks
    {
        private PhotonView pv;
        private string inviteRoomName;
        private bool bCanStart = false;

        public static Action OnJoinRoom = delegate { };
        public static Action OnRoomLeft = delegate { };
        public static Action<List<string>, List<Tuple<int, int>>> OnDisplayPlayers = delegate { };
        public static Action OnMatchSoloPlay = delegate { };
        public static Action OnMatchMultiPlay = delegate { };
        public static Action OnUpdateRoom = delegate { };
        public static Action OnCheckGame = delegate { };
        public static Action<string> OnSendExile = delegate { };
        public static Action OnUpdateDifficulty = delegate { };

        private void Awake()
        {
            PhotonConnector.OnLobbyJoined += HandleLobbyJoined;
            PhotonConnector.OnReturnToLobbyScene += HandleReturnToLobbyScene;
            PhotonChatController.OnRoomFollow += HandleRoomInviteAccept;
            PhotonChatController.OnExile += HandleExile;
            UIReceiveInvite.OnRoomInviteAccept += HandleRoomInviteAccept;
            UIDisplayRoom.OnLeaveRoom += HandleLeaveRoom;
            UISelect.OnGameStart += HandleGameStart;
            UISelect.OnChangeUserData += HandleChangeUserData;
            UISelect.OnChangeCharacter += HandleChangeUserData;
            UISelect.OnUpdateReady += HandleChangeUserData;
            UIPartyOption.OnClickedExit += HandleClickedExit;
            UIChangeName.OnChangeName += HandleChangeName;
            TitleControl.OnTransitionToLobby += HandleTransitionToLobby;

            inviteRoomName = "";
            pv = GetComponent<PhotonView>();
        }

        private void OnDestroy()
        {
            PhotonConnector.OnLobbyJoined -= HandleLobbyJoined;
            PhotonConnector.OnReturnToLobbyScene -= HandleReturnToLobbyScene;
            PhotonChatController.OnRoomFollow -= HandleRoomInviteAccept;
            PhotonChatController.OnExile -= HandleExile;
            UIReceiveInvite.OnRoomInviteAccept -= HandleRoomInviteAccept;
            UIDisplayRoom.OnLeaveRoom -= HandleLeaveRoom;
            UISelect.OnGameStart -= HandleGameStart;
            UISelect.OnChangeUserData -= HandleChangeUserData;
            UISelect.OnChangeCharacter -= HandleChangeUserData;
            UISelect.OnUpdateReady -= HandleChangeUserData;
            UIPartyOption.OnClickedExit -= HandleClickedExit;
            UIChangeName.OnChangeName -= HandleChangeName;
            TitleControl.OnTransitionToLobby -= HandleTransitionToLobby;
        }

        #region Handle Methods
        private void HandleLobbyJoined()
        {
            SetPlayerRole();
            switch(GameManager.Instance.PhotonData.state)
            {
                case PhotonData.EState.NONE:
                    {
                        if (inviteRoomName.IsNullOrEmpty() == false)
                        {
                            PhotonNetwork.JoinRoom(inviteRoomName);
                            inviteRoomName = "";
                        }
                        else
                        {
                            CreatePhotonRoom();
                        }
                    }
                    break;
                case PhotonData.EState.FOLLOW:
                    {
                        if (inviteRoomName.IsNullOrEmpty() == false)
                        {
                            PhotonNetwork.JoinRoom(inviteRoomName);
                            inviteRoomName = "";
                        }
                    }
                    break;
            }
        }

        private void HandleReturnToLobbyScene()
        {
            Debug.Log($"curState = {GameManager.Instance.PhotonData.state}");
            Debug.Log($"InRoom = {PhotonNetwork.InRoom}");
            Debug.Log($"RoomName = {PhotonNetwork.CurrentRoom.Name}");
            Debug.Log($"IsMaster = {PhotonNetwork.IsMasterClient}");

            OnJoinRoom?.Invoke();
            OnUpdateRoom?.Invoke();
            DisplayRoomPlayers();
        }

        private void HandleRoomInviteAccept(string roomName)
        {
            inviteRoomName = roomName;
            if (PhotonNetwork.InRoom)
            {
                if(GameManager.Instance.PhotonData.state == PhotonData.EState.NONE)
                    OnRoomLeft?.Invoke();

                PhotonNetwork.LeaveRoom();
            }
            else
            {
                if (PhotonNetwork.InLobby)
                {
                    PhotonNetwork.JoinRoom(roomName);
                }
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

        private void HandleChangeUserData()
        {
            pv.RPC(nameof(SetLevel), RpcTarget.Others, GameManager.Instance.PlayerData.difficulty);
            SetPlayerRole();
            Debug.Log($"nickName : {PhotonNetwork.NickName}, Job : {GameManager.Instance.PlayerData.job}, Level : {GameManager.Instance.PlayerData.difficulty}");
        }

        private void HandleGameStart()
        {
            switch(GameManager.Instance.PlayerData.difficulty)
            {
                case EDifficulty.Story:
                    {
                        OnMatchSoloPlay?.Invoke();
                    }
                    break;
                case EDifficulty.Normal:
                    {
                        if (IsCanStart())
                        {
                            OnMatchMultiPlay?.Invoke();
                            Debug.Log("You Can Start Game");
                        }
                        else
                        {
                            Debug.Log("Cannot Start Game");
                        }
                    }
                    break;
                case EDifficulty.Hard:
                    {
                        if (IsCanStart())
                        {
                            OnMatchMultiPlay?.Invoke();
                            Debug.Log("You Can Start Game");
                        }
                        else
                        {
                            Debug.Log("Cannot Start Game");
                        }
                    }
                    break;
            }
        }

        private void HandleClickedExit()
        {
            if (PhotonNetwork.InRoom)
                PhotonNetwork.LeaveRoom();
        }

        private void HandleExile()
        {
            if (PhotonNetwork.InRoom)
                PhotonNetwork.LeaveRoom();
        }

        private void HandleChangeName(string newName)
        {
            PhotonNetwork.AuthValues.UserId = newName;
            PhotonNetwork.NickName = newName;

            pv.RPC(nameof(UpdateRoom), RpcTarget.All);
        }

        private void HandleTransitionToLobby()
        {
            if(GameManager.Instance.PhotonData.state == PhotonData.EState.NONE)
                DisplayRoomPlayers();
        }
        #endregion // Handle Methods

        #region PunRPC Methods
        [PunRPC]
        private void SetLevel(EDifficulty newLevel)
        {
            GameManager.Instance.PlayerData.difficulty = newLevel;
            OnUpdateDifficulty?.Invoke();
        }

        [PunRPC]
        private void UpdateRoom()
        {
            DisplayRoomPlayers();
        }
        #endregion // PunRPC Methods

        #region Private Methods
        private void CreatePhotonRoom()
        {
            string roomName = Guid.NewGuid().ToString();
            RoomOptions roomOptions = GetRoomOptions();

            PhotonNetwork.CreateRoom(roomName, roomOptions);
        }

        private RoomOptions GetRoomOptions() // �� �ʱ� ����
        {
            Hashtable roomProperties = new Hashtable()
            {
                {"DEALER",0 },
                {"HEALER",0 },
                {"LEVEL", EDifficulty.Normal }
            };

            RoomOptions ro = new RoomOptions();
            ro.IsOpen = true;
            ro.IsVisible = false;
            ro.PublishUserId = true;
            ro.MaxPlayers = GameManager.Instance.GameMode.maxPlayer;
            ro.CustomRoomProperties = roomProperties;
            ro.CustomRoomPropertiesForLobby = new string[] { "DEALER", "HEALER", "LEVEL" };

            return ro;
        }
        
        private void DisplayRoomPlayers()
        {
            List<string> playerList = new List<string>();
            Dictionary<int, Player> players = PhotonNetwork.CurrentRoom.Players;
            int key = 0;
            List<Tuple<int, int>> playerData = new List<Tuple<int, int>>();

            for (int i = 1; i <= players.Count; key++)
            {
                if (players.ContainsKey(key) == true)
                {
                    playerList.Add(players[key].NickName);

                    int characterNum = 0;
                    int jobNum = 0;

                    if (players[key].CustomProperties.ContainsKey("Character"))
                    {
                        characterNum = (int)players[key].CustomProperties["Character"];
                    }
                    if (players[key].CustomProperties.ContainsKey("Job"))
                    {
                        jobNum = (int)players[key].CustomProperties["Job"];
                    }

                    playerData.Add(new Tuple<int, int>(characterNum, jobNum));

                    i++;
                }
            }

            OnDisplayPlayers?.Invoke(playerList, playerData);
        }
        private void SetPlayerRole()
        {
            PlayerData data = GameManager.Instance.PlayerData;

            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { "Character", (int)data.character } });
            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { "Job", (int)data.job } });
            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { "Level", (int)data.difficulty } });
            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { "Ready", data.bReady } });
        }
        private void UpdateRoomCondition()
        {
            Hashtable roomProperties = PhotonNetwork.CurrentRoom.CustomProperties;
            int curDealers = 0;
            int curHealers = 0;

            Dictionary<int, Player> players = PhotonNetwork.CurrentRoom.Players;
            int key = 0;
            bCanStart = true;

            for (int i = 0; i < players.Count; key++)
            {
                if (players.ContainsKey(key) == true)
                {
                    if (players[key].CustomProperties.ContainsKey("Job"))
                    {
                        int jobNum = (int)players[key].CustomProperties["Job"];

                        switch (jobNum)
                        {
                            case 0:
                                curDealers++;
                                break;
                            case 1:
                                curHealers++;
                                break;
                        }
                    }
                    if(players[key].CustomProperties.ContainsKey("Ready"))
                    {
                        if (!(bool)players[key].CustomProperties["Ready"])
                            bCanStart = false;
                    }
                    i++;
                }
            }

            roomProperties["DEALER"] = curDealers;
            roomProperties["HEALER"] = curHealers;
            roomProperties["LEVEL"] = GameManager.Instance.PlayerData.difficulty;

            PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);

            switch (GameManager.Instance.PhotonData.state)
            {
                case PhotonData.EState.NONE:
                    {
                        DisplayRoomPlayers();
                    }
                    break;
            }

            DebugRoomProperties();
        }
        private bool IsCanStart()
        {
            if (PhotonNetwork.InRoom == false) return false;
            if (!bCanStart) return false;

            int curDealer = (int)PhotonNetwork.CurrentRoom.CustomProperties["DEALER"];
            int curHealer = (int)PhotonNetwork.CurrentRoom.CustomProperties["HEALER"];
            
            if (curDealer > GameManager.Instance.GameMode.MaxDealer) return false;
            if (curHealer > GameManager.Instance.GameMode.MaxHealer) return false;

            return true;
        }
        #endregion // Private Methods

        #region Photon Callbacks
        public override void OnCreatedRoom()
        {
            Debug.Log($"Created Room Successful.\nPhoton RoomName : {PhotonNetwork.CurrentRoom.Name}");
        }
        public override void OnJoinedRoom()
        {
            Debug.Log($"Joined Room Successful.\nPhoton RoomName : {PhotonNetwork.CurrentRoom.Name}");

            switch(GameManager.Instance.PhotonData.state)
            {
                case PhotonData.EState.NONE:
                    {
                        OnJoinRoom?.Invoke();
                        OnUpdateRoom?.Invoke();
                        DisplayRoomPlayers();
                    }
                    break;
            }

            DebugPlayerList();
        }
        public override void OnLeftRoom()
        {
            Debug.Log("You have left Photon Room");
        }
        // RandomRoom Join ���� �� ���� �ݹ� �Լ� ����
        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log($"Join Random Failed {returnCode} : {message}");
        }
        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.Log($"Join Room Failed {returnCode} : {message}");
            switch(GameManager.Instance.PhotonData.state)
            {
                case PhotonData.EState.NONE:
                    {
                        CreatePhotonRoom();
                    }
                    break;
            }
        }
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            Debug.Log($"Another player has joined the room : {newPlayer.NickName}");

            switch (GameManager.Instance.PhotonData.state)
            {
                case PhotonData.EState.NONE:
                    {
                        DisplayRoomPlayers();
                        OnUpdateRoom?.Invoke();
                    }
                    break;
            }
            DebugPlayerList();
        }
        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            Debug.Log($"Player has left the room : {otherPlayer.NickName}");

            UpdateRoomCondition();
            switch (GameManager.Instance.PhotonData.state)
            {
                case PhotonData.EState.NONE:
                    {
                        OnUpdateRoom?.Invoke();
                    }
                    break;
            }
            DebugPlayerList();
        }
        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            Debug.Log($"{targetPlayer} Properties Update");
            UpdateRoomCondition();
            OnCheckGame?.Invoke();
        }
        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            Debug.Log("MasterSwitch");
        }

        #endregion

        #region Debug Methods
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

        private void DebugRoomProperties()
        {
            if (PhotonNetwork.InRoom == false) return;

            int curDealer = (int)PhotonNetwork.CurrentRoom.CustomProperties["DEALER"];
            int curHealer = (int)PhotonNetwork.CurrentRoom.CustomProperties["HEALER"];
            EDifficulty difficulty = (EDifficulty)PhotonNetwork.CurrentRoom.CustomProperties["LEVEL"];

            Debug.Log($"dealer : {curDealer}, healer : {curHealer}, level : ${difficulty}");
        }
        #endregion
    }
}