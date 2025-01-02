using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System;

namespace EverScord
{
    public class PhotonConnector : MonoBehaviourPunCallbacks
    {
        private readonly string version = "1.0"; // 게임 버전 체크

        public static Action GetPhotonFriends = delegate { };
        public static Action OnLobbyJoined = delegate { };

        #region Private Methods
        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            PhotonNetwork.GameVersion = version;            // 버전 할당.
            Debug.Log(PhotonNetwork.SendRate);              // Photon서버와의 통신 횟수를 로그로 찍기. 기본값 : 30, 제대로 통신이 되면 30이 출력
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
            PhotonNetwork.AutomaticallySyncScene = true;    // 씬 동기화. 맨 처음 접속한 사람이 방장
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
