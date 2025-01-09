using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;

namespace EverScord
{
    public class PhotonMatchController : MonoBehaviourPunCallbacks
    {
        private List<RoomInfo> curRoomList;
        private Dictionary<string, PlayerData> playerDatas;

        private void Awake()
        {
            PhotonRoomController.OnMatchSoloPlay += HandleMatchSoloPlay;
            PhotonRoomController.OnMatchMultiPlay += HandleMatchMultiPlay;
            PhotonRoomController.OnJoinedMatch += HandleJoinedMatch;

            playerDatas = new Dictionary<string, PlayerData>();
        }

        private void OnDestroy()
        {
            PhotonRoomController.OnMatchSoloPlay -= HandleMatchSoloPlay;
            PhotonRoomController.OnMatchMultiPlay -= HandleMatchMultiPlay;
            PhotonRoomController.OnJoinedMatch += HandleJoinedMatch;
        }

        #region Handle Methods
        private void HandleMatchSoloPlay()
        {
            // 솔로 플레이인 Story모드는 바로 다른 Scene으로 넘어갈수 있게 구현
            PhotonNetwork.LoadLevel("PhotonTestPlay");
        }

        private void HandleMatchMultiPlay(string jobName, string difficulty)
        {
            // 멀티 플레이인 Normal, Hard는 직업조건에 맞춰 매칭을 할 필요가 있으므로 매칭 대기열에 등록
            // 룸내 인원이 1명이라면 랜덤매칭, 룸내 인원이 2명 이상이라면 RoomOptions를 변경하여 참여가능한 방으로 변경
            // 1명이 랜덤매칭할 경우 랜덤매칭 실패시 (조건에 맞는 방을 찾지 못했을 경우) 새로 Room을 만들고 RoomOptions를 변경하여 참여가능한 방으로 변경
            ChangeToPhotonMatchRoom(jobName, difficulty);
        }
        private void HandleJoinedMatch()
        {
            List<string> players = new List<string>();
            Dictionary<int, Player> playerList = PhotonNetwork.CurrentRoom.Players;
            int key = 0;
            for (int i = 1; i <= playerList.Count; key++)
            {
                if (playerList.ContainsKey(key) == true)
                {
                    players.Add(playerList[key].NickName);
                    i++;
                }
            }
        }
        #endregion

        #region Private Methods
        private void JoinPhotonRandomMatchRoom()
        {
            for (int i = 0; i < curRoomList.Count; i++)
            {
                bool bJoined = PhotonNetwork.JoinRoom(curRoomList[i].Name);
                if (bJoined == true || PhotonNetwork.InRoom == true)
                {
                    break;
                }
            }
        }
        private void ChangeToPhotonMatchRoom(string jobName, string difficulty)
        {
            // 이 시점에 룸내 플레이어의 상태를 확인하고 매칭조건이 결정되어 값을 넘겨줘야함
            // MergeTest
            PhotonNetwork.CurrentRoom.IsVisible = true;

            string[] roomProperties = { "Match", "Difficulty" };
            Hashtable customRoomProperties = new Hashtable()
            {
                { roomProperties[0], jobName },
                { roomProperties[1], difficulty }
            };

            PhotonNetwork.CurrentRoom.SetPropertiesListedInLobby(roomProperties);
            PhotonNetwork.CurrentRoom.SetCustomProperties(customRoomProperties);
        }
        private void SetPlayerData(string name, PlayerData newPlayerData)
        {
            if(playerDatas.ContainsKey(name))
            {
                playerDatas[name] = newPlayerData;
            }
            else
            {
                playerDatas.Add(name, newPlayerData);
            }
        }
        #endregion

        #region Photon Callbacks
        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            curRoomList = roomList;
        }
        #endregion
    }
}