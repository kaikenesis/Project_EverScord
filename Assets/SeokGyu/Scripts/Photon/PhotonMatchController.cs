using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using WebSocketSharp;
using UnityEngine;
using System;

namespace EverScord
{
    public class PhotonMatchController : MonoBehaviourPunCallbacks
    {
        private int maxPlayers = 3;
        private string[] roomProperties = { "Match", "Difficulty" };
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
            GameManager.Instance.userDatas[PhotonNetwork.AuthValues.UserId].curPhotonState = EPhotonState.MATCH;
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

            Hashtable customRoomProperties = GetCustomRoomProperties(true);

            PhotonNetwork.CurrentRoom.SetPropertiesListedInLobby(roomProperties);
            PhotonNetwork.CurrentRoom.SetCustomProperties(customRoomProperties);
        }

        private Hashtable GetCustomRoomProperties(bool bCreateRoom)
        {
            int dealer = 2;
            int healer = 1;
            Dictionary<string, PlayerData> userDatas = GameManager.Instance.userDatas;
            string[] userIDs = UserIDs;

            for (int i = 0; i < userIDs.Length; i++)
            {
                switch (userDatas[userIDs[i]].job)
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
            ELevel level = userDatas[PhotonNetwork.AuthValues.UserId].curLevel;
            string matchRoles = "";
            
            
            // ���� ����� ��Ŀ� ���� ���� ����� �ʿ� ����
            if (healer > 0)
            {
                if (matchRoles.IsNullOrEmpty() == false) matchRoles += ":";
                matchRoles += EJob.HEALER.ToString();
            }
            else
            {
                if (bCreateRoom == false)
                {
                    EJob job = userDatas[PhotonNetwork.AuthValues.UserId].job;
                    matchRoles += job.ToString();
                }
            }

            if (dealer > 0)
            {
                if (matchRoles.IsNullOrEmpty() == false) matchRoles += ":";
                matchRoles += EJob.DEALER.ToString();
            }
            else
            {
                if (bCreateRoom == false)
                {
                    EJob job = userDatas[PhotonNetwork.AuthValues.UserId].job;
                    matchRoles += job.ToString();
                }
            }

            customRoomProperties = new Hashtable()
            {
                { roomProperties[0], matchRoles },
                { roomProperties[1], level.ToString() }
            };

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

        private void SingleMatch(int tryCount)
        {
            string nickName = PhotonNetwork.AuthValues.UserId;
            PlayerData playerData = GameManager.Instance.userDatas[nickName];
            Hashtable customRoomProperties = new Hashtable();

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
                case 1: // �� ������ ���� �ִ� �� Ž�� ( 2�� �õ� ) ex. ����and������ �ʿ��� ��
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
                case 1: // �� ����
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
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            switch (GameManager.Instance.userDatas[PhotonNetwork.AuthValues.UserId].curPhotonState)
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
            switch (GameManager.Instance.userDatas[PhotonNetwork.AuthValues.UserId].curPhotonState)
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
            switch (GameManager.Instance.userDatas[PhotonNetwork.AuthValues.UserId].curPhotonState)
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
            switch (GameManager.Instance.userDatas[PhotonNetwork.AuthValues.UserId].curPhotonState)
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