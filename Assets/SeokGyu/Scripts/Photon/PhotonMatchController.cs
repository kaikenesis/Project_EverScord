using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;

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
            PhotonNetwork.LoadLevel("TestPlayScene");
        }

        private void HandleMatchMultiPlay()
        {
            // ��Ƽ �÷����� Normal, Hard�� �������ǿ� ���� ��Ī�� �� �ʿ䰡 �����Ƿ� ��Ī ��⿭�� ���
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
        private void ChangeToPhotonMatchRoom()
        {
            PhotonNetwork.CurrentRoom.IsVisible = true;
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