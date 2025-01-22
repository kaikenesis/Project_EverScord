using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using System;
using UnityEngine;

namespace EverScord
{
    public class PhotonMatchController : MonoBehaviourPunCallbacks
    {
        private int maxDealers = 2;
        private int maxHealers = 1;
        Dictionary<int, Player> matchPlayers;
        private Hashtable expectedRoomProperties = new Hashtable();
        private PhotonView pv;

        public static Action<string, string> OnFollowRoom = delegate { };
        public static Action OnStateUpdate = delegate { };

        private void Awake()
        {
            pv = GetComponent<PhotonView>();
            PhotonRoomController.OnMatchSoloPlay += HandleMatchSoloPlay;
            PhotonRoomController.OnMatchMultiPlay += HandleMatchMultiPlay;
        }

        private void OnDestroy()
        {
            PhotonRoomController.OnMatchSoloPlay -= HandleMatchSoloPlay;
            PhotonRoomController.OnMatchMultiPlay -= HandleMatchMultiPlay;
        }

        #region Handle Methods
        private void HandleMatchSoloPlay()
        {
            if(PhotonNetwork.InRoom == true)
                PhotonNetwork.LoadLevel("PhotonTestPlay");
        }

        private void HandleMatchMultiPlay()
        {
            // ��Ƽ �÷����� Normal, Hard�� �������ǿ� ���� ��Ī�� �� �ʿ䰡 �����Ƿ� ��Ī ��⿭�� ���
            // ������Ī�� ��� ������Ī ���н� (���ǿ� �´� ���� ã�� ������ ���) ���� Room�� ����� RoomOptions�� �����Ͽ� ��Ī���� ���������� ������ ����
            // ��ġ����ŷ�� �κ񿡼� ����Ǿ����
            if (PhotonNetwork.IsMasterClient == false) return;
            EPhotonState state = GameManager.Instance.userData.curPhotonState;
            if (state == EPhotonState.MATCH || state == EPhotonState.FOLLOW) return;

            pv.RPC("SetStateMatch", RpcTarget.Others);
            GameManager.Instance.userData.curPhotonState = EPhotonState.MATCH;

            if (PhotonNetwork.InRoom == true)
            {
                matchPlayers = PhotonNetwork.CurrentRoom.Players;
                expectedRoomProperties = GetCurrentRoomProperties();
                PhotonNetwork.LeaveRoom();
            }
            else
            {
                expectedRoomProperties = PhotonNetwork.LocalPlayer.CustomProperties;
                PhotonNetwork.JoinRandomRoom(expectedRoomProperties, maxDealers + maxHealers);
            }
        }
        #endregion

        #region Private Methods
        private void CreatePhotonMatchRoom()
        {
            string roomName = Guid.NewGuid().ToString();

            RoomOptions ro = new RoomOptions();
            ro.IsOpen = true;
            ro.IsVisible = true;
            ro.PublishUserId = true;
            ro.MaxPlayers = maxDealers + maxHealers;
            ro.CustomRoomProperties = expectedRoomProperties;
            ro.CustomRoomPropertiesForLobby = new string[] { "DEALER", "HEALER", "LEVEL" };

            PhotonNetwork.CreateRoom(roomName, ro);
        }

        private bool FindRoomForRole(RoomInfo room)
        {
            Debug.Log("FindRoom");

            int roomDealer = (int)room.CustomProperties["DEALER"];
            int roomHealer = (int)room.CustomProperties["HEALER"];
            ELevel roomLevel = (ELevel)room.CustomProperties["LEVEL"];
            Debug.Log($"roomInfo : {roomDealer}, {roomHealer}, {roomLevel}");

            int joinDealer = (int)expectedRoomProperties["DEALER"];
            int joinHealer = (int)expectedRoomProperties["HEALER"];
            ELevel joinLevel = (ELevel)expectedRoomProperties["LEVEL"];
            Debug.Log($"roomInfo : {joinDealer}, {joinHealer}, {joinLevel}");

            if (room.IsVisible == false) return false;
            if (roomLevel != joinLevel) return false;
            if (roomDealer + joinDealer > maxDealers) return false;
            if (roomHealer + joinHealer > maxHealers) return false;

            Debug.Log($"{room.Name}");
            if (matchPlayers.Count != 0)
                FollowRoom(room.Name);
            return PhotonNetwork.JoinRoom(room.Name);
        }
        private Hashtable GetCurrentRoomProperties()
        {
            return PhotonNetwork.CurrentRoom.CustomProperties;
        }
        private void FollowRoom(string roomName)
        {
            // �̹� �κ�� ���� ���� Ž���߱� ������ RPC�� Ȱ���ϱ� ����.
            // �÷��̾� �ʴ�� ������ �������� ����������, �����ϴ� ������ ��ġ���ʰ� �ٷ� �������� ������� ����
            // ù��° ���ڿ� recipient, �ι�° ���ڿ� roomName message
            string message = roomName;
            int key = 0;
            for (int i = 0; i < matchPlayers.Count; key++)
            {
                if(matchPlayers.ContainsKey(key))
                {
                    OnFollowRoom?.Invoke(matchPlayers[key].NickName, message);
                    i++;
                }
            }
        }
        #endregion

        #region Coroutine Methods
        private System.Collections.IEnumerator WaitCreatePhotonMatchRoom()
        {
            yield return new WaitForSeconds(2.0f);

            CreatePhotonMatchRoom();
        }
        #endregion

        #region PunRPC Methods
        [PunRPC]
        private void SetStateMatch()
        {
            GameManager.Instance.userData.curPhotonState = EPhotonState.FOLLOW;
        }
        #endregion

        #region Photon Callbacks
        // �κ� ���ӽ�, ���ο� ���� ������� ���, ���� �����Ǵ� ���, ���� IsOpen���� ��ȭ�� ���
        // ���������� �ִ� �游 �Ѿ��, �κ񿡼��� ȣ�Ⱑ��..
        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            Debug.Log("UpdateRoomList");
            switch (GameManager.Instance.userData.curPhotonState)
            {
                case EPhotonState.MATCH:
                    foreach (RoomInfo room in roomList)
                    {
                        // ���� ���ǰ� �� ������ �´��� Ȯ��
                        if(FindRoomForRole(room) == true)
                            return;
                    }
                    // ��Ī�Ǵ� ���� ������ ���ο� ���� ����
                    Debug.Log("No matching room found.");

                    StartCoroutine(WaitCreatePhotonMatchRoom());
                    break;
            }
        }
        #endregion
    }
}