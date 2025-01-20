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
            // ��Ƽ �÷����� Normal, Hard�� �������ǿ� ���� ��Ī�� �� �ʿ䰡 �����Ƿ� ��Ī ��⿭�� ���
            // �볻 �ο��� 1���̶�� ������Ī, �볻 �ο��� 2�� �̻��̶�� RoomOptions�� �����Ͽ� ���������� ������ ����
            // 1���� ������Ī�� ��� ������Ī ���н� (���ǿ� �´� ���� ã�� ������ ���) ���� Room�� ����� RoomOptions�� �����Ͽ� ���������� ������ ����
            // ��ġ����ŷ�� �κ񿡼� ����Ǿ����; ���پ�

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
            // ���� �ο��� �� ���ӽ��� ���� Ȯ��. ������ ���� ����
            if (PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount == maxPlayers)
            {
                // ������ �ְ� �Ѿ����
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
            // �� ������ �볻 �÷��̾��� ���¸� Ȯ���ϰ� ��Ī������ �����Ǿ� ���� �Ѱ������
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
                case 0: // �� ������ ���� �ִ� �� ���� Ž�� ( 1�� �õ� )
                    {
                        customRoomProperties = new Hashtable()
                        {
                            {roomProperties[0], playerData.job.ToString()},
                            {roomProperties[1], playerData.curLevel.ToString() }
                        };
                        PhotonNetwork.JoinRandomRoom(customRoomProperties, maxPlayers, MatchmakingMode.FillRoom, null, null, UserIDs);
                    }
                    break;
                case 1: // �� ������ ���� �ִ� �� Ž�� ( 2�� �õ� ) ex. ���� ������� ����and������ �ʿ��� ��
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
                case 0: // ��Ƽ�� ���� �ִ� �� ���� Ž�� ( 1�� �õ� )
                    {
                        customRoomProperties = GetCustomRoomProperties(false);
                        PhotonNetwork.JoinRandomRoom(customRoomProperties, maxPlayers, MatchmakingMode.FillRoom, null, null, UserIDs);
                    }
                    break;
                case 1: // �� �� �ִ� ���� �ƹ��͵� ���ٸ� �� ����
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

                            // ���� ���ǰ� �� ������ �´��� Ȯ��
                            if (level == GameManager.Instance.userData.curLevel && job.Contains(GameManager.Instance.userData.job.ToString()))
                            {
                                Debug.Log($"Found a matching room: {room.Name}");
                                PhotonNetwork.JoinRoom(room.Name);  // ��Ī�� �뿡 ����
                                return;
                            }
                        }
                    }
                    // ��Ī�Ǵ� ���� ������ ���ο� ���� ����
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