using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;

namespace EverScord
{
    public class PhotonConnector : MonoBehaviourPunCallbacks
    {
        private readonly string version = "1.0"; // 게임 버전 체크

        public static Action OnLobbyJoined = delegate { };
        public static Action<string, int> OnLogin = delegate { };
        public static Action OnReturnToLobbyScene = delegate { };

        #region Private Methods
        private void Awake()
        {
            Init();
        }

        private void Start()
        {
            if (PhotonNetwork.IsConnected)
            {
                OnReturnToLobbyScene?.Invoke();
            }
        }

        private void Init()
        {
            PhotonNetwork.GameVersion = version;            // 버전 할당.
            PhotonNetwork.SendRate = 60;
            Debug.Log(PhotonNetwork.SendRate);              // Photon서버와의 통신 횟수를 로그로 찍기. 기본값 : 30

            PhotonNetwork.SerializationRate = 30;           // 초당 몇 번 OnPhotonSerialize가 실행되는지. 기본값 : 10
            Debug.Log(PhotonNetwork.SerializationRate);

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
            PhotonNetwork.AutomaticallySyncScene = true;    // 씬 동기화. 맨 처음 접속한 사람이 방장, 방장이 씬 이동시 Room의 멤버들도 함께 이동
            PhotonNetwork.NickName = nickName;

            OnLogin?.Invoke(PhotonNetwork.NickName, GameManager.Instance.PlayerData.money);
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

            OnLobbyJoined?.Invoke();
        }
        #endregion
    }
}
