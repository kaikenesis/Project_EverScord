using UnityEngine;
using System;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using WebSocketSharp;

namespace EverScord
{
    public class PhotonRoomController : MonoBehaviourPunCallbacks
    {
        [SerializeField] private int maxPlayers;
        [SerializeField] private int maxDealers = 2;
        [SerializeField] private int maxHealers = 1;
        [SerializeField] private bool bDebug = false;
        private PhotonView pv;
        private string inviteRoomName;

        public static Action OnJoinRoom = delegate { };
        public static Action OnRoomLeft = delegate { };
        public static Action<List<string>> OnDisplayPlayers = delegate { };
        public static Action OnMatchSoloPlay = delegate { };
        public static Action OnMatchMultiPlay = delegate { };
        public static Action OnUpdateRoom = delegate { };
        public static Action OnCheckGame = delegate { };
        public static Action<string> OnSendExile = delegate { };

        private void Awake()
        {
            PhotonConnector.OnLobbyJoined += HandleLobbyJoined;
            PhotonChatController.OnRoomFollow += HandleRoomInviteAccept;
            PhotonChatController.OnExile += HandleExile;
            UIInvite.OnRoomInviteAccept += HandleRoomInviteAccept;
            UIDisplayRoom.OnLeaveRoom += HandleLeaveRoom;
            UISelect.OnChangeUserData += HandleChangeUserData;
            UISelect.OnGameStart += HandleGameStart;
            UIPartyOption.OnClickedExit += HandleClickedExit;

            inviteRoomName = "";
            pv = GetComponent<PhotonView>();
        }

        private void OnDestroy()
        {
            PhotonConnector.OnLobbyJoined -= HandleLobbyJoined;
            PhotonChatController.OnRoomFollow -= HandleRoomInviteAccept;
            PhotonChatController.OnExile -= HandleExile;
            UIInvite.OnRoomInviteAccept -= HandleRoomInviteAccept;
            UIDisplayRoom.OnLeaveRoom -= HandleLeaveRoom;
            UISelect.OnChangeUserData -= HandleChangeUserData;
            UISelect.OnGameStart -= HandleGameStart;
            UIPartyOption.OnClickedExit -= HandleClickedExit;
        }

        #region Handle Methods
        private void HandleLobbyJoined()
        {
            SetPlayerRole();
            switch(GameManager.Instance.userData.curPhotonState)
            {
                case EPhotonState.NONE:
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
                case EPhotonState.FOLLOW:
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

        private void HandleRoomInviteAccept(string roomName)
        {
            inviteRoomName = roomName;
            if (PhotonNetwork.InRoom)
            {
                if(GameManager.Instance.userData.curPhotonState == EPhotonState.NONE)
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
            pv.RPC("SetLevel", RpcTarget.Others, GameManager.Instance.userData.curLevel);
            SetPlayerRole();
            Debug.Log($"nickName : {GameManager.Instance.userName}, Job : {GameManager.Instance.userData.job}, Level : {GameManager.Instance.userData.curLevel}");
        }

        private void HandleGameStart()
        {
            switch(GameManager.Instance.userData.curLevel)
            {
                case ELevel.STORY:
                    OnMatchSoloPlay?.Invoke();
                    break;
                case ELevel.NORMAL:
                    if(PhotonNetwork.CurrentRoom.PlayerCount < PhotonNetwork.CurrentRoom.MaxPlayers)
                    {
                        OnMatchMultiPlay?.Invoke();
                    }
                    else
                    {
                        if(IsCanStart())
                        {
                            Debug.Log("You Can Start Game");
                        }
                        else
                        {
                            Debug.Log("Cannot Start Game");
                        }
                    }
                    break;
                case ELevel.HARD:
                    if (PhotonNetwork.CurrentRoom.PlayerCount < PhotonNetwork.CurrentRoom.MaxPlayers)
                    {
                        OnMatchMultiPlay?.Invoke();
                    }
                    else
                    {
                        if (IsCanStart())
                        {
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
        #endregion

        #region PunRPC Methods
        [PunRPC]
        private void SetLevel(ELevel newLevel)
        {
            GameManager.Instance.userData.curLevel = newLevel;
        }
        #endregion

        #region Private Methods
        private void CreatePhotonRoom()
        {
            string roomName = Guid.NewGuid().ToString();
            RoomOptions roomOptions = GetRoomOptions();

            PhotonNetwork.CreateRoom(roomName, roomOptions);
        }

        private RoomOptions GetRoomOptions() // 룸 초기 생성
        {
            Hashtable roomProperties = new Hashtable()
            {
                {"DEALER",0 },
                {"HEALER",0 },
                {"LEVEL", ELevel.NORMAL }
            };

            RoomOptions ro = new RoomOptions();
            ro.IsOpen = true;
            ro.IsVisible = false;
            ro.PublishUserId = true;
            ro.MaxPlayers = maxDealers + maxHealers;
            ro.CustomRoomProperties = roomProperties;
            ro.CustomRoomPropertiesForLobby = new string[] { "DEALER", "HEALER", "LEVEL" };

            return ro;
        }
        
        private void DisplayRoomPlayers()
        {
            List<string> playerList = new List<string>();
            Dictionary<int, Player> players = PhotonNetwork.CurrentRoom.Players;
            int key = 0;

            for (int i = 1; i <= players.Count; key++)
            {
                if (players.ContainsKey(key) == true)
                {
                    playerList.Add(players[key].NickName);
                    i++;
                }
            }

            OnDisplayPlayers?.Invoke(playerList);
        }
        private void SetPlayerRole()
        {
            PlayerData data = GameManager.Instance.userData;
            switch(data.job)
            {
                case EJob.DEALER:
                    PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { "Job", "DEALER" } });
                    break;
                case EJob.HEALER:
                    PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { "Job", "HEALER" } });
                    break;
            }

            switch(data.curLevel)
            {
                case ELevel.NORMAL:
                    PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { "Level", "NORMAL" } });
                    break;
                case ELevel.HARD:
                    PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { "Level", "HARD" } });
                    break;
            }
        }
        private void UpdateRoomCondition()
        {
            Hashtable roomProperties = PhotonNetwork.CurrentRoom.CustomProperties;
            int curDealers = 0;
            int curHealers = 0;

            Dictionary<int, Player> players = PhotonNetwork.CurrentRoom.Players;
            int key = 0;

            for (int i = 0; i < players.Count; key++)
            {
                if (players.ContainsKey(key) && players[key].CustomProperties.ContainsKey("Job"))
                {
                    string playerJob = (string)players[key].CustomProperties["Job"];

                    if (playerJob == "DEALER")
                        curDealers++;
                    else if (playerJob == "HEALER")
                        curHealers++;

                    i++;
                }
            }
            roomProperties["DEALER"] = curDealers;
            roomProperties["HEALER"] = curHealers;
            roomProperties["LEVEL"] = GameManager.Instance.userData.curLevel;

            PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);
            
            DebugRoomProperties();
        }
        private bool IsCanStart()
        {
            if (PhotonNetwork.InRoom == false) return false;

            int curDealer = (int)PhotonNetwork.CurrentRoom.CustomProperties["DEALER"];
            int curHealer = (int)PhotonNetwork.CurrentRoom.CustomProperties["HEALER"];

            if (curDealer < maxDealers) return false;
            if (curHealer < maxHealers) return false;

            return true;
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

            switch(GameManager.Instance.userData.curPhotonState)
            {
                case EPhotonState.NONE:
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
        // RandomRoom Join 실패 시 오류 콜백 함수 실행
        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log($"Join Random Failed {returnCode} : {message}");
        }
        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.Log($"Join Room Failed {returnCode} : {message}");
            switch(GameManager.Instance.userData.curPhotonState)
            {
                case EPhotonState.NONE:
                    {
                        CreatePhotonRoom();
                    }
                    break;
            }
        }
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            Debug.Log($"Another player has joined the room : {newPlayer.NickName}");

            switch (GameManager.Instance.userData.curPhotonState)
            {
                case EPhotonState.NONE:
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
            switch (GameManager.Instance.userData.curPhotonState)
            {
                case EPhotonState.NONE:
                    {
                        DisplayRoomPlayers();
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
            switch(GameManager.Instance.userData.curPhotonState)
            {
                case EPhotonState.NONE:
                    {
                        // 현재 방 인원이 max일때 직업조건을 확인하고 플레이가 불가능하면 시스템메시지 출력 ( 일단은 반응x으로)
                    }
                    break;
            }
        }
        public override void OnMasterClientSwitched(Player newMasterClient)
        {
        }

        #endregion

        #region Debug Methods
        private void OnGUI()
        {
            if (bDebug == true)
            {
                if (GUI.Button(new Rect(400, 0, 150, 60), "JoinRoom"))
                {
                    CreatePhotonRoom();
                }
                
                if (GUI.Button(new Rect(600, 0, 150, 60), "Play"))
                {
                    PhotonNetwork.LoadLevel("PhotonTestPlay");
                }
            }
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

        private void DebugRoomProperties()
        {
            if (PhotonNetwork.InRoom == false) return;

            int curDealer = (int)PhotonNetwork.CurrentRoom.CustomProperties["DEALER"];
            int curHealer = (int)PhotonNetwork.CurrentRoom.CustomProperties["HEALER"];
            ELevel level = (ELevel)PhotonNetwork.CurrentRoom.CustomProperties["LEVEL"];

            Debug.Log($"dealer : {curDealer}, healer : {curHealer}, level : ${level}");
        }
        #endregion
    }
}