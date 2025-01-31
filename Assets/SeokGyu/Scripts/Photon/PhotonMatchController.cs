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
            // 멀티 플레이인 Normal, Hard는 직업조건에 맞춰 매칭을 할 필요가 있으므로 매칭 대기열에 등록
            // 랜덤매칭할 경우 랜덤매칭 실패시 (조건에 맞는 방을 찾지 못했을 경우) 새로 Room을 만들고 RoomOptions를 변경하여 매칭으로 참여가능한 방으로 생성
            // 매치메이킹은 로비에서 진행되어야함
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
            // 이미 로비로 나와 룸을 탐색했기 때문에 RPC는 활용하기 힘듬.
            // 플레이어 초대와 동일한 수단으로 진행하지만, 수락하는 과정은 거치지않고 바로 데려오는 방식으로 진행
            // 첫번째 인자에 recipient, 두번째 인자에 roomName message
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
        // 로비에 접속시, 새로운 룸이 만들어질 경우, 룸이 삭제되는 경우, 룸의 IsOpen값이 변화할 경우
        // 변동사항이 있는 방만 넘어옴, 로비에서만 호출가능..
        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            Debug.Log("UpdateRoomList");
            switch (GameManager.Instance.userData.curPhotonState)
            {
                case EPhotonState.MATCH:
                    foreach (RoomInfo room in roomList)
                    {
                        // 룸의 조건과 내 조건이 맞는지 확인
                        if(FindRoomForRole(room) == true)
                            return;
                    }
                    // 매칭되는 룸이 없으면 새로운 룸을 생성
                    Debug.Log("No matching room found.");

                    StartCoroutine(WaitCreatePhotonMatchRoom());
                    break;
            }
        }
        #endregion
    }
}