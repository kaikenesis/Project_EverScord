using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

namespace EverScord
{
    public class UIDisplayParty : MonoBehaviour
    {
        private void Awake()
        {
            if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
            {
                //Dictionary<int, Player> players = PhotonNetwork.CurrentRoom.Players;
                for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
                {
                    
                }
            }
        }
    }
}
