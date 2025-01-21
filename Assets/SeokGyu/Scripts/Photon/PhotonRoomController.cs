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
        [SerializeField] private int maxPlayers;
        private bool bMatch = false;
        [SerializeField] private bool bDebug = false;

        public static Action OnJoinRoom = delegate { };
        public static Action<bool> OnRoomStatusChange = delegate { };
        public static Action OnRoomLeft = delegate { };
        public static Action<List<string>> OnDisplayPlayers = delegate { };
        public static Action OnMatchSoloPlay = delegate { };
        public static Action<int> OnMatchMultiPlay = delegate { };
        public static Action OnJoinedMatch = delegate { };
        public static Action OnUpdateMatchRoom = delegate { };

        private void Awake()
        {
            UIInvite.OnRoomInviteAccept += HandleRoomInviteAccept;
            PhotonConnector.OnLobbyJoined += HandleLobbyJoined;
            UIDisplayRoom.OnLeaveRoom += HandleLeaveRoom;

            PlayerPrefs.SetString("PHOTONROOM", "");
        }

        private void OnDestroy()
        {
            UIInvite.OnRoomInviteAccept -= HandleRoomInviteAccept;
            PhotonConnector.OnLobbyJoined -= HandleLobbyJoined;
            UIDisplayRoom.OnLeaveRoom -= HandleLeaveRoom;
        }

        #region Handle Methods
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
                //CreatePhotonRoom();
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
            ro.MaxPlayers = maxPlayers;
            ro.CustomRoomProperties = roomProperties;
            ro.CustomRoomPropertiesForLobby = new string[] { "DEALER", "HEALER", "LEVEL" };

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
                    players += $"{playerList[key].NickName},\n";
                    i++;
                }
            }
            Debug.Log($"Current Room Players: {players}");
        }
        private void DisplayRoomPlayers()
        {
            List<string> players = new List<string>();
            Dictionary<int, Player> playerList = PhotonNetwork.CurrentRoom.Players;
            int key = 0;
            for (int i = 1; i <= playerList.Count; key++)
            {
                if (playerList.ContainsKey(key) == true)
                {
                    string[] data = playerList[key].NickName.Split("|");
                    players.Add(data[0]);
                    i++;
                }
            }

            OnDisplayPlayers?.Invoke(players);
        }
        private void SetPlayerRole()
        {
            Hashtable roomProperties = PhotonNetwork.CurrentRoom.CustomProperties;
            int curDealers = (int)roomProperties["DEALER"];
            int curHealers = (int)roomProperties["HEALER"];

            PlayerData data = GameManager.Instance.userData;
            if(data.job.ToString() == "DEALER" && curDealers < 2)
            {
                roomProperties["DEALER"] = curDealers + 1;
                PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { "Job", "DEALER" } });
            }
            else if(data.job.ToString() == "HEALER" && curHealers < 1)
            {
                roomProperties["HEALER"] = curDealers + 1;
                PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable { { "Job", "HEALER" } });
            }

            PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);
        }
        private void UpdateRoom(Player otherPlayer)
        {
            Hashtable roomProperties = PhotonNetwork.CurrentRoom.CustomProperties;

            if(otherPlayer.CustomProperties.ContainsKey("Job"))
            {
                string playerJob = (string)otherPlayer.CustomProperties["Job"];
                if(playerJob == "DEALER")
                {
                    roomProperties["DEALER"] = (int)roomProperties["DEALER"] - 1;
                }
                else if(playerJob == "HEALER")
                {
                    roomProperties["HEALER"] = (int)roomProperties["HEALER"] - 1;
                }

                PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);
            }
        }
        #endregion

        #region Public Methods
        public void OnClickedSoloPlay()
        {
            OnMatchSoloPlay?.Invoke();
        }

        public void OnClickedMultiPlay()
        {
            bMatch = true;
            OnMatchMultiPlay?.Invoke(0);
        }
        #endregion

        #region Photon Callbacks
        public override void OnCreatedRoom()
        {
            Debug.Log($"Created Room Successful.\nPhoton RoomName : {PhotonNetwork.CurrentRoom.Name}");

            SetPlayerRole();
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

            switch (GameManager.Instance.userData.curPhotonState)
            {
                case EPhotonState.NONE:
                    {
                        DisplayRoomPlayers();
                    }
                    break;
            }
        }
        // RandomRoom Join 실패 시 오류 콜백 함수 실행
        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log($"Join Random Failed {returnCode} : {message}");
            
            //CreatePhotonRoom();
        }
        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.Log($"Join Room Failed {returnCode} : {message}");
            
            //CreatePhotonRoom();
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
            UpdateRoom(newPlayer);
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
            UpdateRoom(otherPlayer);
            DebugPlayerList();
        }
        public override void OnMasterClientSwitched(Player newMasterClient)
        {
        }

        #endregion

        private void OnGUI()
        {
            if (bDebug == true)
            {
                if(GUI.Button(new Rect(400, 0, 150, 60), "JoinRoom"))
                {
                    CreatePhotonRoom();
                }

                if (GUI.Button(new Rect(600, 0, 150, 60), "Dealer"))
                {
                    GameManager.Instance.userData.job = EJob.DEALER;
                    GameManager.Instance.SetUserName(EJob.DEALER, GameManager.Instance.userData.curLevel);
                    Debug.Log(PlayerPrefs.GetString("USERNAME"));
                }
                if (GUI.Button(new Rect(600, 60, 150, 60), "Healer"))
                {
                    GameManager.Instance.userData.job = EJob.HEALER;
                    GameManager.Instance.SetUserName(EJob.HEALER, GameManager.Instance.userData.curLevel);
                    Debug.Log(PlayerPrefs.GetString("USERNAME"));
                }

                if (GUI.Button(new Rect(900, 0, 150, 60), "Normal"))
                {
                    GameManager.Instance.userData.curLevel = ELevel.NORMAL;
                    GameManager.Instance.SetUserName(GameManager.Instance.userData.job, ELevel.NORMAL);
                    Debug.Log(PlayerPrefs.GetString("USERNAME"));
                }
                if (GUI.Button(new Rect(900, 60, 150, 60), "Hard"))
                {
                    GameManager.Instance.userData.curLevel = ELevel.HARD;
                    GameManager.Instance.SetUserName(GameManager.Instance.userData.job, ELevel.HARD);
                    Debug.Log(PlayerPrefs.GetString("USERNAME"));
                }
            }
        }
    }
}