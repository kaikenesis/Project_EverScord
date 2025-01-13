using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System;
using ExitGames.Client.Photon.StructWrapping;

namespace EverScord
{
    public class PhotonConnector : MonoBehaviourPunCallbacks
    {
        [SerializeField] private bool bDebug = false;
        private readonly string version = "1.0"; // ���� ���� üũ

        public static Action GetPhotonFriends = delegate { };
        public static Action OnLobbyJoined = delegate { };

        #region Private Methods
        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            PhotonNetwork.GameVersion = version;            // ���� �Ҵ�.
            Debug.Log(PhotonNetwork.SendRate);              // Photon�������� ��� Ƚ���� �α׷� ���. �⺻�� : 30, ����� ����� �Ǹ� 30�� ���
            PhotonLogin.OnConnectToPhoton += HandleConnectToPhoton;
        }

        private void OnDestroy()
        {
            PhotonLogin.OnConnectToPhoton -= HandleConnectToPhoton;
        }

        private void ConnectToPhoton(string nickName)
        {
            Debug.Log($"Connect to Photon as {nickName}");
            PhotonNetwork.AuthValues = new AuthenticationValues(nickName);
            PhotonNetwork.AutomaticallySyncScene = true;    // �� ����ȭ. �� ó�� ������ ����� ����, ������ �� �̵��� Room�� ����鵵 �Բ� �̵�
            PhotonNetwork.NickName = nickName;
            PhotonNetwork.ConnectUsingSettings();
        }
        #endregion

        #region Handle Methods
        private void HandleConnectToPhoton(string nickName)
        {
            ConnectToPhoton(nickName);
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

            //GetPhotonFriends?.Invoke();
            OnLobbyJoined?.Invoke();
        }
        #endregion

        private void OnGUI()
        {
            if(bDebug == true)
            {
                if (GUI.Button(new Rect(600, 0, 150, 60), "Dealer"))
                {
                    Debug.Log("Set Job Dealer");
                }
                if (GUI.Button(new Rect(600, 60, 150, 60), "Healer"))
                {
                    Debug.Log("Set Job Healer");
                }
            }
        }
    }
}
