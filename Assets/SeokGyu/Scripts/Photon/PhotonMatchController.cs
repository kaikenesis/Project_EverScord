using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using Unity.VisualScripting;

namespace EverScord
{
    public class PhotonMatchController : MonoBehaviourPunCallbacks
    {
        private List<RoomInfo> curRoomList;
        private Dictionary<string, PlayerData> playerDatas;
        private string[] roomProperties = { "Match", "Difficulty" };

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
            PhotonRoomController.OnJoinedMatch -= HandleJoinedMatch;
        }

        #region Handle Methods
        private void HandleMatchSoloPlay()
        {
            if(PhotonNetwork.InRoom == true)
                PhotonNetwork.LoadLevel("PhotonTestPlay");
        }

        private void HandleMatchMultiPlay(string jobName, string difficulty)
        {
            // 멀티 플레이인 Normal, Hard는 직업조건에 맞춰 매칭을 할 필요가 있으므로 매칭 대기열에 등록
            // 룸내 인원이 1명이라면 랜덤매칭, 룸내 인원이 2명 이상이라면 RoomOptions를 변경하여 참여가능한 방으로 변경
            // 1명이 랜덤매칭할 경우 랜덤매칭 실패시 (조건에 맞는 방을 찾지 못했을 경우) 새로 Room을 만들고 RoomOptions를 변경하여 참여가능한 방으로 변경
            if(PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                string nickName = PlayerPrefs.GetString("USERNAME");
                PlayerData playerData = GameManager.Instance.userDatas[nickName];

                Hashtable customRoomProperties = new Hashtable()
                {
                    { roomProperties[0], playerData.job.ToString() },
                    { roomProperties[1], playerData.curLevel.ToString() }
                };
                PhotonNetwork.JoinRandomRoom(customRoomProperties, 3);
            }
            else if(PhotonNetwork.CurrentRoom.PlayerCount < 3)
            {
                ChangeToPhotonMatchRoom();
            }
            else if(PhotonNetwork.CurrentRoom.PlayerCount == 3)
            {
                HandleJoinedMatch();
            }
        }
        private void HandleJoinedMatch()
        {
            // 현재 인원수 및 게임시작 조건 확인. 충족시 게임 시작
            if (PhotonNetwork.CurrentRoom.PlayerCount == 3)
            {
                // 딜레이 주고서 넘어가도록
                PhotonNetwork.LoadLevel("PhotonTestPlay");
            }
        }
        #endregion

        #region Private Methods
        private void ChangeToPhotonMatchRoom()
        {
            // 이 시점에 룸내 플레이어의 상태를 확인하고 매칭조건이 결정되어 값을 넘겨줘야함
            PhotonNetwork.CurrentRoom.IsVisible = true;

            Hashtable customRoomProperties = GetCustomRoomProperties();

            PhotonNetwork.CurrentRoom.SetPropertiesListedInLobby(roomProperties);
            PhotonNetwork.CurrentRoom.SetCustomProperties(customRoomProperties);
        }
        private Hashtable GetCustomRoomProperties()
        {
            int dealer = 2;
            int healer = 1;
            Dictionary<string, PlayerData> userDatas = GameManager.Instance.userDatas;
            List<string> playerNickNames = GetPlayerNickNames();
            for (int i = 0; i < playerNickNames.Count; i++)
            {
                switch(userDatas[playerNickNames[i]].job)
                {
                    case EJob.DEALER:
                        dealer--;
                        break;
                    case EJob.HEALER:
                        healer--;
                        break;
                }
            }

            Hashtable customRoomProperties;
            ELevel level = GameManager.Instance.userDatas[PlayerPrefs.GetString("USERNAME")].curLevel;
            if (dealer > 0)
            {
                customRoomProperties = new Hashtable()
                {
                    { roomProperties[0], EJob.HEALER.ToString() },
                    { roomProperties[1], level.ToString() }
                };
            }
            else
            {
                customRoomProperties = new Hashtable()
                {
                    { roomProperties[0], EJob.DEALER.ToString() },
                    { roomProperties[1], level.ToString() }
                };
            }

            return customRoomProperties;
        }
        private List<string> GetPlayerNickNames()
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

            return players;
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