using System;
using UnityEngine;

namespace EverScord
{
    public class UILobby : MonoBehaviour
    {
        private void Awake()
        {
            PhotonConnector.OnLobbyJoined += HandleLobbyJoined;
        }

        private void OnDestroy()
        {
            PhotonConnector.OnLobbyJoined -= HandleLobbyJoined;
        }

        private void HandleLobbyJoined()
        {
            VisibleUI();
        }

        private void VisibleUI()
        {
            gameObject.SetActive(true);
        }
    }
}
