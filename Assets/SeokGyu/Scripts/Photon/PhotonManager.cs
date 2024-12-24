using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.Demo.Cockpit;
using TMPro;

namespace EverScord
{
    public class PhotonManager : MonoBehaviourPunCallbacks
    {
        /*
            실행
            -> userId를 입력하고 로그인
            -> 로비룸에서 대기. 이때 매칭을 넣을수도, 초대를 받을수도, 초대를 할수도 있음.
            -> 
        */
        private readonly string version = "1.0"; // 게임 버전 체크
        private string userId;

        [SerializeField] private TMP_InputField userInputField;
        [SerializeField] private TMP_InputField roomInputField;
        [SerializeField] private TMP_InputField searchIDInputField;
        [SerializeField] private GameObject LogInUI;

        [SerializeField] int maxPlayerCount;

        
        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            PhotonNetwork.AutomaticallySyncScene = true;    // 씬 동기화. 맨 처음 접속한 사람이 방장
            PhotonNetwork.GameVersion = version;            // 버전 할당.
            Debug.Log(PhotonNetwork.SendRate);              // Photon서버와의 통신 횟수를 로그로 찍기. 기본값 : 30, 제대로 통신이 되면 30이 출력
        }

        public void OnLogIn()
        {
            // 마스터서버에 접속
            if (PhotonNetwork.IsConnected == true)
            {
                Debug.Log("Already connected to Master");
                return;
            }
            if (userInputField.text == null || userInputField.text == "")
            {
                Debug.Log("You need to input UserID");
                return;
            }

            userId = userInputField.text;
            PhotonNetwork.ConnectUsingSettings();
            LogInUI.SetActive(false);
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("Connected to Master");                   // 마스터 서버에 접속이 되었는지 디버깅
            Debug.Log($"In Lobby = {PhotonNetwork.InLobby}");   // 로비에 들어와 있으면 True, 아니면 False 반환. Master 서버에는 접속했지만 로비는 아니므로 False.
            PhotonNetwork.JoinLobby();                          // 로비 접속
        }

        public override void OnJoinedLobby()
        {
            Debug.Log($"In Lobby = {PhotonNetwork.InLobby}");
            // 방 접속 방법은 두 가지. 1 랜던 매치메이킹, 2 선택된 방 접속

            //PhotonNetwork.JoinRandomRoom();
        }

        public void InviteUser()
        {
            if (searchIDInputField.text == null || searchIDInputField.text == "")
            {
                Debug.Log("You need to input UserID");
                return;
            }

            PhotonNetwork.FindFriends(new string[1] { searchIDInputField.text });
            
        }

        public void JoinOrCreatePrivateRoom(string nameEveryFriendKnows)
        {
            Debug.Log("JoinOrCreatePrivateRoom : " + nameEveryFriendKnows);
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.IsVisible = false;
            PhotonNetwork.JoinOrCreateRoom(nameEveryFriendKnows, roomOptions, null);
        }

        // 방 생성이 되지 않았으면 오류 콜백 함수 실행
        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log($"JoinRandom Failed {returnCode} : {message}");

        }

        // 제대로 방이 있다면 다음의 콜백 함수 호출
        public override void OnCreatedRoom()
        {
            Debug.Log("Created Room");
            Debug.Log($"Room Name : {PhotonNetwork.CurrentRoom.Name}");
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
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.LoadLevel("TestRoom");
            }
        }

        private void Start()
        {
            SetInfo();
        }

        private void SetInfo()
        {
            // 유저 ID 랜덤 설정, 20명까지 밖에 못들어오므로 1~21 설정. -> 00은 한 자리도 두 자리로 만들기 위해서
            userId = PlayerPrefs.GetString("USER_ID", $"USER_{Random.Range(1, 21):00}");
            PhotonNetwork.NickName = userId;
        }

        private string SetRoomName()
        {
            if (string.IsNullOrEmpty(roomInputField.text))
            {
                roomInputField.text = $"ROOM_{Random.Range(1, 101):000}";
            }
            return roomInputField.text;
        }

        // 개인랜덤매칭 or 파티랜덤매칭(파티 : 초대, 배열로 저장한 뒤 전달?) or 플레이어초대(userId 검색, 방코드 등등)?

        // 개인랜덤매칭
        public void OnMakeRoom()
        {
            RoomOptions roomOption = new RoomOptions();
            roomOption.MaxPlayers = 3;
            roomOption.IsOpen = true;    // false시 join 불가능. ex) 게임시작 후 다른 플레이어가 도중참가를 원치 않을때 사용
            roomOption.IsVisible = true; // false시 RandomJoin이 안됨
                                         // 고정된 값이 아닌 유저가 타이핑한 값을 받아옴
            PhotonNetwork.CreateRoom(SetRoomName(), roomOption);
        }
    }
}
