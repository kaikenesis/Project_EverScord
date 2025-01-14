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
            // ��Ƽ �÷����� Normal, Hard�� �������ǿ� ���� ��Ī�� �� �ʿ䰡 �����Ƿ� ��Ī ��⿭�� ���
            // �볻 �ο��� 1���̶�� ������Ī, �볻 �ο��� 2�� �̻��̶�� RoomOptions�� �����Ͽ� ���������� ������ ����
            // 1���� ������Ī�� ��� ������Ī ���н� (���ǿ� �´� ���� ã�� ������ ���) ���� Room�� ����� RoomOptions�� �����Ͽ� ���������� ������ ����
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
            // ���� �ο��� �� ���ӽ��� ���� Ȯ��. ������ ���� ����
            if (PhotonNetwork.CurrentRoom.PlayerCount == 3)
            {
                // ������ �ְ� �Ѿ����
                PhotonNetwork.LoadLevel("PhotonTestPlay");
            }
        }
        #endregion

        #region Private Methods
        private void ChangeToPhotonMatchRoom()
        {
            // �� ������ �볻 �÷��̾��� ���¸� Ȯ���ϰ� ��Ī������ �����Ǿ� ���� �Ѱ������
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