using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System;

namespace EverScord
{
    public class PhotonConnector : MonoBehaviourPunCallbacks
    {
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
            PhotonNetwork.AutomaticallySyncScene = true;    // �� ����ȭ. �� ó�� ������ ����� ����
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
    }
}
