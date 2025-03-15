using UnityEngine;
using UnityEngine.UI;

namespace EverScord
{
    public class UILogin : MonoBehaviour
    {
        [SerializeField] private GameObject warninngText;
        [SerializeField] private Outline warninngOutline;
        private ToggleObject toggleObject;

        private void Awake()
        {
            PhotonConnector.OnLobbyJoined += HandleLobbyJoined;
            PhotonConnector.OnReturnToLobbyScene += HandleLobbyJoined;
            PhotonLogin.OnLoginError += HandleLoginError;

            toggleObject = GetComponent<ToggleObject>();
        }

        private void OnDestroy()
        {
            PhotonConnector.OnLobbyJoined -= HandleLobbyJoined;
            PhotonConnector.OnReturnToLobbyScene -= HandleLobbyJoined;
            PhotonLogin.OnLoginError -= HandleLoginError;
        }

        private void HandleLobbyJoined()
        {
            HiddenUI();
        }

        private void HandleLoginError()
        {
            warninngOutline.enabled = true;
            warninngText.SetActive(true);
        }

        private void HiddenUI()
        {
            warninngOutline.enabled = false;
            warninngText.SetActive(false);
            toggleObject.OnToggleObjects();
        }
    }
}
