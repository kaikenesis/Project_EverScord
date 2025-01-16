using UnityEngine;
using System;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Collections;

namespace EverScord
{
    public enum EMatchMode
    {
        NONE,
        SINGLE,
        MULTI
    }

    public class PhotonRoomController : MonoBehaviourPunCallbacks
    {
        [SerializeField] private int maxPlayers;
        private bool bMatch = false;
        [SerializeField] private bool bDebug = false;

        public static Action OnJoinRoom = delegate { };
        public static Action<bool> OnRoomStatusChange = delegate { };
        public static Action OnRoomLeft = delegate { };
        public static Action<Player> OnOtherPlayerLeftRoom = delegate { };
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

            PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
        }

        private RoomOptions GetRoomOptions()
        {
            RoomOptions ro = new RoomOptions();
            ro.IsOpen = true;
            ro.IsVisible = false;
            ro.PublishUserId = true;
            ro.MaxPlayers = maxPlayers;

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
        }
        public override void OnJoinedRoom()
        {
            Debug.Log($"Joined Room Successful.\nPhoton RoomName : {PhotonNetwork.CurrentRoom.Name}");

            switch(GameManager.Instance.userDatas[PhotonNetwork.AuthValues.UserId].curPhotonState)
            {
                case EPhotonState.NONE:
                    {
                        DebugPlayerList();
                        OnJoinRoom?.Invoke();
                        DisplayRoomPlayers();
                    }
                    break;
                case EPhotonState.MATCH:
                    {
                        OnJoinedMatch?.Invoke();
                    }
                    break;
            }

            // 매칭중에는 막아야함
            if(true)
            {
                DebugPlayerList();
                OnJoinRoom?.Invoke();
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
                //DisplayRoomPlayers();
            }
        }
        // RandomRoom Join 실패 시 오류 콜백 함수 실행
        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log($"Join Random Failed {returnCode} : {message}");
            switch (GameManager.Instance.userDatas[PhotonNetwork.AuthValues.UserId].curPhotonState)
            {
                case EPhotonState.NONE:
                    {
                    }
                    break;
                case EPhotonState.MATCH:
                    {
                    }
                    break;
            }
            //CreatePhotonRoom();
        }
        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.Log($"Join Room Failed {returnCode} : {message}");
            if (bMatch)
            {

            }
            else
            {
                //CreatePhotonRoom();
            }
        }
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            Debug.Log($"Another player has joined the room : {newPlayer.NickName}");

            switch (GameManager.Instance.userDatas[PhotonNetwork.AuthValues.UserId].curPhotonState)
            {
                case EPhotonState.NONE:
                    {
                    }
                    break;
                case EPhotonState.MATCH:
                    {
                    }
                    break;
            }

            if (bMatch)
            {
                OnUpdateMatchRoom?.Invoke();
            }
            else
            {
                DisplayRoomPlayers();
            }
            DebugPlayerList();
        }
        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            Debug.Log($"Player has left the room : {otherPlayer.NickName}");

            switch (GameManager.Instance.userDatas[PhotonNetwork.AuthValues.UserId].curPhotonState)
            {
                case EPhotonState.NONE:
                    {
                    }
                    break;
                case EPhotonState.MATCH:
                    {
                    }
                    break;
            }

            if (bMatch)
            {
                OnUpdateMatchRoom?.Invoke();
            }
            else
            {
                OnOtherPlayerLeftRoom?.Invoke(otherPlayer);
                DisplayRoomPlayers();
            }
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
                    GameManager.Instance.userDatas[PhotonNetwork.AuthValues.UserId].job = EJob.DEALER;
                    Debug.Log(GameManager.Instance.userDatas[PhotonNetwork.AuthValues.UserId].job.ToString());
                }
                if (GUI.Button(new Rect(600, 60, 150, 60), "Healer"))
                {
                    GameManager.Instance.userDatas[PhotonNetwork.AuthValues.UserId].job = EJob.HEALER;
                    Debug.Log(GameManager.Instance.userDatas[PhotonNetwork.AuthValues.UserId].job.ToString());
                }

                if (GUI.Button(new Rect(900, 0, 150, 60), "Normal"))
                {
                    GameManager.Instance.userDatas[PhotonNetwork.AuthValues.UserId].curLevel = ELevel.NORMAL;
                    Debug.Log(GameManager.Instance.userDatas[PhotonNetwork.AuthValues.UserId].curLevel.ToString());
                }
                if (GUI.Button(new Rect(900, 60, 150, 60), "Hard"))
                {
                    GameManager.Instance.userDatas[PhotonNetwork.AuthValues.UserId].curLevel = ELevel.HARD;
                    Debug.Log(GameManager.Instance.userDatas[PhotonNetwork.AuthValues.UserId].curLevel.ToString());
                }
            }
        }
    }
}