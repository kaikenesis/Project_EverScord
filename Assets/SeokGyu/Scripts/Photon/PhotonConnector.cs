using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System;

namespace EverScord
{
    public class PhotonConnector : MonoBehaviourPunCallbacks
    {
        /*
            실행
            -> userId를 입력하고 로그인
            -> 로비룸에서 대기. 이때 매칭을 넣을수도, 초대를 받을수도, 초대를 할수도 있음.
            -> 
        */
        // 개인랜덤매칭 or 파티랜덤매칭(파티 : 초대, 배열로 저장한 뒤 전달?) or 플레이어초대(userId 검색, 방코드 등등)?

        private readonly string version = "1.0"; // 게임 버전 체크
        //private static PhotonConnector instance;
        //public static PhotonConnector Instance
        //{
        //    get
        //    {
        //        if (instance == null)
        //        {
        //            GameObject newGameObject = new GameObject("PhotonManager");
        //            instance = newGameObject.AddComponent<PhotonConnector>();
        //        }
        //        return instance;
        //    }
        //}

        public static Action GetPhotonFriends = delegate { };
        public static Action OnLobbyJoined = delegate { };

        #region Private Methods

        private void Awake()
        {
            Init();
        }

        private void OnDestroy()
        {
            UIInvite.OnRoomInviteAccept -= HandleRoomInviteAccept;
        }

        private void Init()
        {
            PhotonNetwork.GameVersion = version;            // 버전 할당.
            Debug.Log(PhotonNetwork.SendRate);              // Photon서버와의 통신 횟수를 로그로 찍기. 기본값 : 30, 제대로 통신이 되면 30이 출력
            UIInvite.OnRoomInviteAccept += HandleRoomInviteAccept;

            //if(instance != null && instance != this)
            //{
            //    Destroy(this.gameObject);
            //    return;
            //}
            //instance = this;
            
            //DontDestroyOnLoad(this.gameObject);
        }

        private void ConnectToPhoton(string nickName)
        {
            Debug.Log($"Connect to Photon as {nickName}");
            PhotonNetwork.AuthValues = new AuthenticationValues(nickName);
            PhotonNetwork.AutomaticallySyncScene = true;    // 씬 동기화. 맨 처음 접속한 사람이 방장
            PhotonNetwork.NickName = nickName;
            PhotonNetwork.ConnectUsingSettings();
        }

        private void HandleRoomInviteAccept(string roomName)
        {
            //PlayerPrefs.SetString("PHOTONROOM", roomName);
            //if (PhotonNetwork.InRoom)
            //{
            //    PhotonNetwork.LeaveRoom();
            //}
            //else
            //{
            //    if (PhotonNetwork.InLobby)
            //    {
            //        JoinPlayerRoom();
            //    }
            //}
        }

        private void JoinPlayerRoom()
        {
            string roomName = PlayerPrefs.GetString("PHOTONROOM");
            PlayerPrefs.SetString("PHOTONROOM", "");
            PhotonNetwork.JoinRoom(roomName);
        }

        #endregion

        #region Public Methods

        public void LoginPhoton()
        {
            string nickName = PlayerPrefs.GetString("USERNAME");
            if (string.IsNullOrEmpty(nickName)) return;
            ConnectToPhoton(nickName);
        }

        public void CreatePhotonRoom(string roomName)
        {
            //Debug.Log("JoinOrCreatePrivateRoom : " + roomName);

            //RoomOptions roomOptions = new RoomOptions();
            //roomOptions.IsOpen = true;
            //roomOptions.IsVisible = true;
            //roomOptions.MaxPlayers = 4;
            //PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
        }

        public void OnCreateRoomClicked(string roomName)
        {
            CreatePhotonRoom(roomName);
        }

        #endregion

        #region Photon Callbacks

        public override void OnConnectedToMaster()
        {
            Debug.Log("Connected to Master");
            if (!PhotonNetwork.InLobby)
            {
                PhotonNetwork.JoinLobby();
            }
        }

        public override void OnJoinedLobby()
        {
            Debug.Log($"In Lobby = {PhotonNetwork.InLobby}");

            GetPhotonFriends?.Invoke();
            OnLobbyJoined?.Invoke();
            string roomName = PlayerPrefs.GetString("PHOTONROOM");
            if (!string.IsNullOrEmpty(roomName))
            {
                JoinPlayerRoom();
            }
        }

        // 방에 들어왔을때 콜백 함수
        public override void OnJoinedRoom()
        {
            Debug.Log($"In Room = {PhotonNetwork.InRoom}");
            int pCount = PhotonNetwork.CurrentRoom.PlayerCount;
            Debug.Log($"Player Count = {pCount}");

            // 접속한 사용자 닉네임 확인
            foreach (var player in PhotonNetwork.CurrentRoom.Players)
            {
                // 플레이어 닉네임, 유저의 고유값 확인
                Debug.Log($"플레이어 닉네임 : {player.Value.NickName}\n 유저 고유값 : {player.Value.ActorNumber}");
            }

            // 마스터 클라이언트인 경우 게임 씬 로딩
            //if (PhotonNetwork.IsMasterClient)
            //{
            //    PhotonNetwork.LoadLevel("TestPlayScene");
            //}
        }

        #endregion
    }
}
