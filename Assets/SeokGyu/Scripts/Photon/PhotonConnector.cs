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

        private readonly string version = "1.0"; // ���� ���� üũ
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
            PhotonNetwork.GameVersion = version;            // ���� �Ҵ�.
            Debug.Log(PhotonNetwork.SendRate);              // Photon�������� ��� Ƚ���� �α׷� ���. �⺻�� : 30, ����� ����� �Ǹ� 30�� ���
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
            PhotonNetwork.AutomaticallySyncScene = true;    // �� ����ȭ. �� ó�� ������ ����� ����
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
            //    PhotonNetwork.LoadLevel("TestPlayScene");
            //}
        }

        #endregion
    }
}
