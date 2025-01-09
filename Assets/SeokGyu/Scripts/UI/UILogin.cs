using System;
using UnityEngine;

namespace EverScord
{
    public class UILogin : MonoBehaviour
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
            HiddenUI();
        }

        private void HiddenUI()
        {
            gameObject.SetActive(false);
        }
    }
}
