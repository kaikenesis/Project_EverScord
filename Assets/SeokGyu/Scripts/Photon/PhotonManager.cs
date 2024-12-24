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
            ����
            -> userId�� �Է��ϰ� �α���
            -> �κ�뿡�� ���. �̶� ��Ī�� ��������, �ʴ븦 ��������, �ʴ븦 �Ҽ��� ����.
            -> 
        */
        private readonly string version = "1.0"; // ���� ���� üũ
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
            PhotonNetwork.AutomaticallySyncScene = true;    // �� ����ȭ. �� ó�� ������ ����� ����
            PhotonNetwork.GameVersion = version;            // ���� �Ҵ�.
            Debug.Log(PhotonNetwork.SendRate);              // Photon�������� ��� Ƚ���� �α׷� ���. �⺻�� : 30, ����� ����� �Ǹ� 30�� ���
        }

        public void OnLogIn()
        {
            // �����ͼ����� ����
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
            Debug.Log("Connected to Master");                   // ������ ������ ������ �Ǿ����� �����
            Debug.Log($"In Lobby = {PhotonNetwork.InLobby}");   // �κ� ���� ������ True, �ƴϸ� False ��ȯ. Master �������� ���������� �κ�� �ƴϹǷ� False.
            PhotonNetwork.JoinLobby();                          // �κ� ����
        }

        public override void OnJoinedLobby()
        {
            Debug.Log($"In Lobby = {PhotonNetwork.InLobby}");
            // �� ���� ����� �� ����. 1 ���� ��ġ����ŷ, 2 ���õ� �� ����

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

        // �� ������ ���� �ʾ����� ���� �ݹ� �Լ� ����
        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log($"JoinRandom Failed {returnCode} : {message}");

        }

        // ����� ���� �ִٸ� ������ �ݹ� �Լ� ȣ��
        public override void OnCreatedRoom()
        {
            Debug.Log("Created Room");
            Debug.Log($"Room Name : {PhotonNetwork.CurrentRoom.Name}");
        }

        // �濡 �������� �ݹ� �Լ�
        public override void OnJoinedRoom()
        {
            Debug.Log($"In Room = {PhotonNetwork.InRoom}");
            int pCount = PhotonNetwork.CurrentRoom.PlayerCount;
            Debug.Log($"Player Count = {pCount}");

            // ������ ����� �г��� Ȯ��
            foreach (var player in PhotonNetwork.CurrentRoom.Players)
            {
                // �÷��̾� �г���, ������ ������ Ȯ��
                Debug.Log($"�÷��̾� �г��� : {player.Value.NickName}\n ���� ������ : {player.Value.ActorNumber}");
            }

            // ������ Ŭ���̾�Ʈ�� ��� ���� �� �ε�
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
            // ���� ID ���� ����, 20����� �ۿ� �������Ƿ� 1~21 ����. -> 00�� �� �ڸ��� �� �ڸ��� ����� ���ؼ�
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

        // ���η�����Ī or ��Ƽ������Ī(��Ƽ : �ʴ�, �迭�� ������ �� ����?) or �÷��̾��ʴ�(userId �˻�, ���ڵ� ���)?

        // ���η�����Ī
        public void OnMakeRoom()
        {
            RoomOptions roomOption = new RoomOptions();
            roomOption.MaxPlayers = 3;
            roomOption.IsOpen = true;    // false�� join �Ұ���. ex) ���ӽ��� �� �ٸ� �÷��̾ ���������� ��ġ ������ ���
            roomOption.IsVisible = true; // false�� RandomJoin�� �ȵ�
                                         // ������ ���� �ƴ� ������ Ÿ������ ���� �޾ƿ�
            PhotonNetwork.CreateRoom(SetRoomName(), roomOption);
        }
    }
}
