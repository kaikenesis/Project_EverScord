using UnityEngine;
using System;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

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
        public static Action<Player> OnOtherPlayerLeftRoom = delegate { };
        public static Action<List<string>> OnDisplayPlayers = delegate { };
        public static Action OnMatchSoloPlay = delegate { };
        public static Action<string, string> OnMatchMultiPlay = delegate { };
        public static Action OnJoinedMatch = delegate { };

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
                GameManager.Instance.userDatas.Add(PlayerPrefs.GetString("USERNAME"), new PlayerData());
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

        private void OnGUI()
        {
            if (bDebug == true)
            {
                if (GUI.Button(new Rect(600, 0, 150, 60), "Dealer"))
                {
                    GameManager.Instance.userDatas[PlayerPrefs.GetString("USERNAME")].job = EJob.DEALER;
                    Debug.Log(GameManager.Instance.userDatas[PlayerPrefs.GetString("USERNAME")].job.ToString());
                }
                if (GUI.Button(new Rect(600, 60, 150, 60), "Healer"))
                {
                    GameManager.Instance.userDatas[PlayerPrefs.GetString("USERNAME")].job = EJob.HEALER;
                    Debug.Log(GameManager.Instance.userDatas[PlayerPrefs.GetString("USERNAME")].job.ToString());
                }

                if (GUI.Button(new Rect(900, 0, 150, 60), "Normal"))
                {
                    GameManager.Instance.userDatas[PlayerPrefs.GetString("USERNAME")].curLevel = ELevel.NORMAL;
                    Debug.Log(GameManager.Instance.userDatas[PlayerPrefs.GetString("USERNAME")].curLevel.ToString());
                }
                if (GUI.Button(new Rect(900, 60, 150, 60), "Hard"))
                {
                    GameManager.Instance.userDatas[PlayerPrefs.GetString("USERNAME")].curLevel = ELevel.HARD;
                    Debug.Log(GameManager.Instance.userDatas[PlayerPrefs.GetString("USERNAME")].curLevel.ToString());
                }
            }
        }
    }
}