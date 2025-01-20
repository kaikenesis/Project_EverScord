using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using WebSocketSharp;
using System;
using UnityEngine;

namespace EverScord
{
    public class PhotonMatchController : MonoBehaviourPunCallbacks
    {
        private int maxPlayers = 3;
        private string[] roomProperties = { "Job", "Difficulty" };
        private string[] roleList = { "HEALER", "DEALER", "HEALER:DEALER" };
        private string[] UserIDs;
        private bool bMatching = false;
        private int tryCount = 0;

        private void Awake()
        {
            PhotonRoomController.OnMatchSoloPlay += HandleMatchSoloPlay;
            PhotonRoomController.OnMatchMultiPlay += HandleMatchMultiPlay;
            PhotonRoomController.OnJoinedMatch += HandleJoinedMatch;
            PhotonRoomController.OnUpdateMatchRoom += HandleUpdateMatchRoom;
        }

        private void OnDestroy()
        {
            PhotonRoomController.OnMatchSoloPlay -= HandleMatchSoloPlay;
            PhotonRoomController.OnMatchMultiPlay -= HandleMatchMultiPlay;
            PhotonRoomController.OnJoinedMatch -= HandleJoinedMatch;
            PhotonRoomController.OnUpdateMatchRoom += HandleUpdateMatchRoom;
        }

        #region Handle Methods
        private void HandleMatchSoloPlay()
        {
            if(PhotonNetwork.InRoom == true)
                PhotonNetwork.LoadLevel("PhotonTestPlay");
        }

        private void HandleMatchMultiPlay(int count)
        {
            // 멀티 플레이인 Normal, Hard는 직업조건에 맞춰 매칭을 할 필요가 있으므로 매칭 대기열에 등록
            // 룸내 인원이 1명이라면 랜덤매칭, 룸내 인원이 2명 이상이라면 RoomOptions를 변경하여 참여가능한 방으로 변경
            // 1명이 랜덤매칭할 경우 랜덤매칭 실패시 (조건에 맞는 방을 찾지 못했을 경우) 새로 Room을 만들고 RoomOptions를 변경하여 참여가능한 방으로 변경
            // 매치메이킹은 로비에서 진행되어야함; 에바야

            tryCount = count;
            GameManager.Instance.userData.curPhotonState = EPhotonState.MATCH;
            bMatching = true;
            if (PhotonNetwork.InRoom == true)
            {
                UserIDs = GetRoomUserIDs();
                PhotonNetwork.LeaveRoom();
            }
            else
            {
                if (UserIDs.Length == 1)
                {
                    SingleMatch(tryCount);
                }
                else if (UserIDs.Length < maxPlayers)
                {
                    GroupMatch(tryCount);
                }
            }
            
            HandleJoinedMatch();
        }

        private void HandleJoinedMatch()
        {
            // 현재 인원수 및 게임시작 조건 확인. 충족시 게임 시작
            if (PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount == maxPlayers)
            {
                // 딜레이 주고서 넘어가도록
                PhotonNetwork.LoadLevel("PhotonTestPlay");
            }
        }

        private void HandleUpdateMatchRoom()
        {
            if (PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient)
            {
                UpdatePhotonMatchRoom();
            }
        }
        #endregion

        #region Private Methods
        private void CreatePhotonMatchRoom()
        {
            string roomName = Guid.NewGuid().ToString();
            RoomOptions roomOptions = GetRoomOptions();

            PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default, UserIDs);
        }

        private RoomOptions GetRoomOptions()
        {
            RoomOptions ro = new RoomOptions();
            ro.IsOpen = true;
            ro.IsVisible = true;
            ro.PublishUserId = true;
            ro.MaxPlayers = maxPlayers;
            ro.CustomRoomPropertiesForLobby = roomProperties;
            ro.CustomRoomProperties = GetCustomRoomProperties(true);

            return ro;
        }

        private void UpdatePhotonMatchRoom()
        {
            // 이 시점에 룸내 플레이어의 상태를 확인하고 매칭조건이 결정되어 값을 넘겨줘야함
            if(PhotonNetwork.CurrentRoom.PlayerCount < maxPlayers)
                PhotonNetwork.CurrentRoom.IsVisible = true;
            else
                PhotonNetwork.CurrentRoom.IsVisible = false;

            UserIDs = GetRoomUserIDs();
            Hashtable customRoomProperties = GetCustomRoomProperties(true);

            PhotonNetwork.CurrentRoom.SetPropertiesListedInLobby(roomProperties);
            PhotonNetwork.CurrentRoom.SetCustomProperties(customRoomProperties);
        }

        private Hashtable GetCustomRoomProperties(bool bCreateRoom)
        {
            int dealer = 2;
            int healer = 1;
            string[] userData = UserIDs;

            for (int i = 0; i < UserIDs.Length; i++)
            {
                userData = UserIDs[i].Split('|');

                if (userData[1] == EJob.DEALER.ToString())
                    dealer--;
                else if (userData[1] == EJob.HEALER.ToString())
                    healer--;
            }

            Hashtable customRoomProperties = new Hashtable();
            string matchRoles = "";

            if(bCreateRoom == true)
            {
                if (healer > 0)
                {
                    if (matchRoles.IsNullOrEmpty() == false) matchRoles += "|";
                    matchRoles += EJob.HEALER.ToString();
                }
                if (dealer > 0)
                {
                    if (matchRoles.IsNullOrEmpty() == false) matchRoles += "|";
                    matchRoles += EJob.DEALER.ToString();
                }
            }
            else
            {
                if(UserIDs.Length == 1)
                {
                    matchRoles = roleList[2];
                }
                else
                {
                    if (healer != 1)
                    {
                        matchRoles = roleList[2];
                    }
                    else 
                    {
                        matchRoles = roleList[1];
                    }
                }
            }

            Debug.Log("new roomRole : " + matchRoles);

            customRoomProperties.Add(roomProperties[0], matchRoles);
            customRoomProperties.Add(roomProperties[1], GameManager.Instance.userData.curLevel);

            return customRoomProperties;
        }
        
        private List<string> GetRoomPlayersNickName()
        {
            List<string> players = new List<string>();
            Dictionary<int, Player> playerList = PhotonNetwork.CurrentRoom.Players;
            int key = 1;
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

        private string[] GetRoomUserIDs()
        {
            List<string> players = GetRoomPlayersNickName();
            string[] userIDs = new string[players.Count];
            for(int i = 0;i < players.Count; i++)
            {
                userIDs[i] = players[i];
            }

            return userIDs;
        }
        private void FindRoomForRole()
        {
            Hashtable expectedRoomProperties = new Hashtable()
            {
                {"DEALER", 0 },
                {"HEALER", 0 }
            };
            int curDealer = 0;
            int curHealer = 0;

            Dictionary<int, Player> players = PhotonNetwork.CurrentRoom.Players;
            string playerJob = "";
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].CustomProperties.ContainsKey("Job"))
                {
                    playerJob = (string)players[i].CustomProperties["Job"];
                    if(playerJob == "DEALER")
                    {
                        expectedRoomProperties["DEALER"] = curDealer + 1;
                    }
                    else if(playerJob == "HEALER")
                    {
                        expectedRoomProperties["HEALER"] = curHealer + 1;
                    }
                }
            }

            //PhotonView.RPC("FollowLeaderToRoom", RpcTarget.All);
        }
        private void SingleMatch(int tryCount)
        {
            PlayerData playerData = GameManager.Instance.userData;
            Hashtable customRoomProperties = new Hashtable();
            customRoomProperties.Add("Job", playerData.job.ToString());
            customRoomProperties.Add("Difficulty", playerData.curLevel);

            switch (tryCount)
            {
                case 0: // 내 직업만 들어갈수 있는 방 먼저 탐색 ( 1차 시도 )
                    {
                        customRoomProperties = new Hashtable()
                        {
                            {roomProperties[0], playerData.job.ToString()},
                            {roomProperties[1], playerData.curLevel.ToString() }
                        };
                        PhotonNetwork.JoinRandomRoom(customRoomProperties, maxPlayers, MatchmakingMode.FillRoom, null, null, UserIDs);
                    }
                    break;
                case 1: // 내 직업이 들어갈수 있는 방 탐색 ( 2차 시도 ) ex. 내가 딜러라면 딜러and힐러가 필요한 방
                    {
                        customRoomProperties = GetCustomRoomProperties(false);
                        PhotonNetwork.JoinRandomRoom(customRoomProperties, maxPlayers, MatchmakingMode.FillRoom, null, null, UserIDs);
                    }
                    break;
                case 2:
                    {
                        CreatePhotonMatchRoom();
                    }
                    break;
            }
        }

        private void GroupMatch(int tryCount)
        {
            Hashtable customRoomProperties = new Hashtable();

            switch (tryCount)
            {
                case 0: // 파티가 들어갈수 있는 방 먼저 탐색 ( 1차 시도 )
                    {
                        customRoomProperties = GetCustomRoomProperties(false);
                        PhotonNetwork.JoinRandomRoom(customRoomProperties, maxPlayers, MatchmakingMode.FillRoom, null, null, UserIDs);
                    }
                    break;
                case 1: // 들어갈 수 있는 방이 아무것도 없다면 방 생성
                    {
                        CreatePhotonMatchRoom();
                    }
                    break;
            }
        }
        #endregion

        #region Photon Callbacks
        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            base.OnRoomListUpdate(roomList);

            switch (GameManager.Instance.userData.curPhotonState)
            {
                case EPhotonState.MATCH:

                    foreach (RoomInfo room in roomList)
                    {
                        // roomProperties[0] = job, roomProperties[1] = Difficulty
                        if (room.CustomProperties.ContainsKey(roomProperties[0]) && room.CustomProperties.ContainsKey(roomProperties[1]))
                        {
                            string job = (string)room.CustomProperties["Job"];
                            ELevel level = (ELevel)room.CustomProperties["Difficulty"];

                            // 룸의 조건과 내 조건이 맞는지 확인
                            if (level == GameManager.Instance.userData.curLevel && job.Contains(GameManager.Instance.userData.job.ToString()))
                            {
                                Debug.Log($"Found a matching room: {room.Name}");
                                PhotonNetwork.JoinRoom(room.Name);  // 매칭된 룸에 참가
                                return;
                            }
                        }
                    }
                    // 매칭되는 룸이 없으면 새로운 룸을 생성
                    Debug.Log("No matching room found. Creating a new room...");

                    break;
            }
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            switch (GameManager.Instance.userData.curPhotonState)
            {
                case EPhotonState.MATCH:
                    {
                        tryCount++;
                        HandleMatchMultiPlay(tryCount);
                    }
                    break;
            }
        }

        public override void OnJoinedLobby()
        {
            switch (GameManager.Instance.userData.curPhotonState)
            {
                case EPhotonState.MATCH:
                    {
                        HandleMatchMultiPlay(tryCount);
                    }
                    break;
            }
        }
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            switch (GameManager.Instance.userData.curPhotonState)
            {
                case EPhotonState.MATCH:
                    {
                        HandleUpdateMatchRoom();
                    }
                    break;
            }
        }
        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            switch (GameManager.Instance.userData.curPhotonState)
            {
                case EPhotonState.MATCH:
                    {
                        HandleUpdateMatchRoom();
                    }
                    break;
            }
        }
        #endregion
    }
}