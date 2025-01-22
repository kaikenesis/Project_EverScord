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
        private string inviteRoomName;

        public static Action OnJoinRoom = delegate { };
        public static Action OnRoomLeft = delegate { };
        public static Action<List<string>> OnDisplayPlayers = delegate { };
        public static Action OnMatchSoloPlay = delegate { };
        public static Action OnMatchMultiPlay = delegate { };

        private void Awake()
        {
            PhotonConnector.OnLobbyJoined += HandleLobbyJoined;
            PhotonChatController.OnRoomFollow += HandleRoomInviteAccept;
            UIInvite.OnRoomInviteAccept += HandleRoomInviteAccept;
            UIDisplayRoom.OnLeaveRoom += HandleLeaveRoom;
            UIJobSelect.OnChangeJob += HandleChangeJob;

            inviteRoomName = "";
        }

        

        private void OnDestroy()
        {
            PhotonConnector.OnLobbyJoined -= HandleLobbyJoined;
            PhotonChatController.OnRoomFollow -= HandleRoomInviteAccept;
            UIInvite.OnRoomInviteAccept -= HandleRoomInviteAccept;
            UIDisplayRoom.OnLeaveRoom -= HandleLeaveRoom;
            UIJobSelect.OnChangeJob -= HandleChangeJob;
        }

        #region Handle Methods
        private void HandleLobbyJoined()
        {
            SetPlayerRole();
            if(inviteRoomName.IsNullOrEmpty() == false)
            {
                PhotonNetwork.JoinRoom(inviteRoomName);
                inviteRoomName = "";
            }

            //else
            //{
            //    //CreatePhotonRoom();
            //}
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

        private void HandleChangeJob()
        {
            SetPlayerRole();
            Debug.Log($"nickName : {GameManager.Instance.name}, Job : {GameManager.Instance.userData.job}, Level : {GameManager.Instance.userData.curLevel}");
        }
        #endregion

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

            PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);
            
            DebugRoomProperties();
        }
        #endregion

        #region Public Methods
        public void OnClickedSoloPlay()
        {
            OnMatchSoloPlay?.Invoke();
        }

        public void OnClickedMultiPlay()
        {
            OnMatchMultiPlay?.Invoke();
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
        }
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            Debug.Log($"Another player has joined the room : {newPlayer.NickName}");

            switch (GameManager.Instance.userData.curPhotonState)
            {
                case EPhotonState.NONE:
                    {
                        DisplayRoomPlayers();
                    }
                    break;
            }
            DebugPlayerList();
        }
        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            Debug.Log($"Player has left the room : {otherPlayer.NickName}");

            switch (GameManager.Instance.userData.curPhotonState)
            {
                case EPhotonState.NONE:
                    {
                        DisplayRoomPlayers();
                    }
                    break;
            }
            UpdateRoomCondition();
            DebugPlayerList();
        }
        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            Debug.Log($"{targetPlayer} Properties Update");
            UpdateRoomCondition();
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

                if (GUI.Button(new Rect(900, 0, 150, 60), "Normal"))
                {
                    GameManager.Instance.userData.curLevel = ELevel.NORMAL;
                    SetPlayerRole();
                    Debug.Log($"nickName : {GameManager.Instance.name}, Job : {GameManager.Instance.userData.job}, Level : {GameManager.Instance.userData.curLevel}");
                }
                if (GUI.Button(new Rect(900, 60, 150, 60), "Hard"))
                {
                    GameManager.Instance.userData.curLevel = ELevel.HARD;
                    SetPlayerRole();
                    Debug.Log($"nickName : {GameManager.Instance.name}, Job : {GameManager.Instance.userData.job}, Level : {GameManager.Instance.userData.curLevel}");
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