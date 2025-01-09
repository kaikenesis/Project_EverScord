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
            // �ַ� �÷����� Story���� �ٷ� �ٸ� Scene���� �Ѿ�� �ְ� ����
            PhotonNetwork.LoadLevel("PhotonTestPlay");
        }

        private void HandleMatchMultiPlay(string jobName, string difficulty)
        {
            // ��Ƽ �÷����� Normal, Hard�� �������ǿ� ���� ��Ī�� �� �ʿ䰡 �����Ƿ� ��Ī ��⿭�� ���
            // �볻 �ο��� 1���̶�� ������Ī, �볻 �ο��� 2�� �̻��̶�� RoomOptions�� �����Ͽ� ���������� ������ ����
            // 1���� ������Ī�� ��� ������Ī ���н� (���ǿ� �´� ���� ã�� ������ ���) ���� Room�� ����� RoomOptions�� �����Ͽ� ���������� ������ ����
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
            // �� ������ �볻 �÷��̾��� ���¸� Ȯ���ϰ� ��Ī������ �����Ǿ� ���� �Ѱ������
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