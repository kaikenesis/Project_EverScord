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
            ����
            -> userId�� �Է��ϰ� �α���
            -> �κ�뿡�� ���. �̶� ��Ī�� ��������, �ʴ븦 ��������, �ʴ븦 �Ҽ��� ����.
            -> 
        */
        // ���η�����Ī or ��Ƽ������Ī(��Ƽ : �ʴ�, �迭�� ������ �� ����?) or �÷��̾��ʴ�(userId �˻�, ���ڵ� ���)?

        public static Action GetPhotonFriends = delegate { };
        private readonly string version = "1.0"; // ���� ���� üũ

        //[SerializeField] int maxPlayerCount;

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
            PhotonNetwork.GameVersion = version;            // ���� �Ҵ�.
            Debug.Log(PhotonNetwork.SendRate);              // Photon�������� ��� Ƚ���� �α׷� ���. �⺻�� : 30, ����� ����� �Ǹ� 30�� ���
            UIInvite.OnRoomInviteAccept += HandleRoomInviteAccept;
        }

        private void ConnectToPhoton(string nickName)
        {
            Debug.Log($"Connect to Photon as {nickName}");
            PhotonNetwork.AuthValues = new AuthenticationValues(nickName);
            PhotonNetwork.AutomaticallySyncScene = true; // �� ����ȭ. �� ó�� ������ ����� ����
            PhotonNetwork.NickName = nickName;
            PhotonNetwork.ConnectUsingSettings();
        }

        private void HandleRoomInviteAccept(string roomName)
        {
            PlayerPrefs.SetString("PHOTONROOM", roomName);
            if (PhotonNetwork.InRoom)
            {
                PhotonNetwork.LeaveRoom();
            }
            else
            {
                if (PhotonNetwork.InLobby)
                {
                    JoinPlayerRoom();
                }
            }
        }

        private void JoinPlayerRoom()
        {
            string roomName = PlayerPrefs.GetString("PHOTONROOM");
            PlayerPrefs.SetString("PHOTONROOM", "");
            PhotonNetwork.JoinRoom(roomName);
        }

        //private string SetRoomName()
        //{
        //    if (string.IsNullOrEmpty(roomInputField.text))
        //    {
        //        roomInputField.text = $"ROOM_{UnityEngine.Random.Range(1, 101):000}";
        //    }
        //    return roomInputField.text;
        //}

        #endregion

        #region Public Methods

        public void LoginPhoton()
        {
            string nickName = PlayerPrefs.GetString("USERNAME");
            if (string.IsNullOrEmpty(nickName)) return;
            ConnectToPhoton(nickName);
        }

        //public void InviteUser()
        //{
        //    if (searchIDInputField.text == null || searchIDInputField.text == "")
        //    {
        //        Debug.Log("You need to input UserID");
        //        return;
        //    }

        //    PhotonNetwork.FindFriends(new string[1] { searchIDInputField.text });

        //}

        public void CreatePhotonRoom(string roomName)
        {
            Debug.Log("JoinOrCreatePrivateRoom : " + roomName);

            RoomOptions roomOptions = new RoomOptions();
            roomOptions.IsOpen = true;
            roomOptions.IsVisible = true;
            roomOptions.MaxPlayers = 4;
            PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
        }

        // ���η�����Ī
        //public void OnMakeRoom()
        //{
        //    RoomOptions roomOption = new RoomOptions();
        //    roomOption.MaxPlayers = 3;
        //    roomOption.IsOpen = true;    // false�� join �Ұ���. ex) ���ӽ��� �� �ٸ� �÷��̾ ���������� ��ġ ������ ���
        //    roomOption.IsVisible = true; // false�� RandomJoin�� �ȵ�
        //                                 // ������ ���� �ƴ� ������ Ÿ������ ���� �޾ƿ�
        //    PhotonNetwork.CreateRoom(SetRoomName(), roomOption);
        //}

        public void OnCreateRoomClicked(string roomName)
        {
            CreatePhotonRoom(roomName);
        }

        #endregion

        #region Photon Callbacks

        public override void OnConnectedToMaster()
        {
            Debug.Log("Connected to Master");                   // ������ ������ ������ �Ǿ����� �����
            Debug.Log($"In Lobby = {PhotonNetwork.InLobby}");   // �κ� ���� ������ True, �ƴϸ� False ��ȯ. Master �������� ���������� �κ�� �ƴϹǷ� False.
            if (!PhotonNetwork.InLobby)
            {
                PhotonNetwork.JoinLobby();                          // �κ� ����
            }
        }

        public override void OnJoinedLobby()
        {
            Debug.Log($"In Lobby = {PhotonNetwork.InLobby}");
            // �� ���� ����� �� ����. 1 ���� ��ġ����ŷ, 2 ���õ� �� ����

            GetPhotonFriends?.Invoke();
            string roomName = PlayerPrefs.GetString("PHOTONROOM");
            if (!string.IsNullOrEmpty(roomName))
            {
                JoinPlayerRoom();
            }
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
            //if (PhotonNetwork.IsMasterClient)
            //{
            //    PhotonNetwork.LoadLevel("TestRoom");
            //}
        }

        #endregion
    }
}
