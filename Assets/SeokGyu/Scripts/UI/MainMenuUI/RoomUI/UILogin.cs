using System;
using DG.Tweening;
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
            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            ShowLoginUI(true);
        }

        private void OnDestroy()
        {
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

            ShowLoginUI(false);
            GameManager.Instance.TitleController.TransitionLobby();
        }

        public void ShowLoginUI(bool state)
        {
            if (state)
            {
                DOTween.Rewind(ConstStrings.TWEEN_LOGIN_IN);
                DOTween.Play(ConstStrings.TWEEN_LOGIN_IN);

                SoundManager.Instance.PlaySound(ConstStrings.SFX_UI_POPUP_1);
            }
            else
            {
                DOTween.Rewind(ConstStrings.TWEEN_LOGIN_OUT);
                DOTween.Play(ConstStrings.TWEEN_LOGIN_OUT);

                // Tween OnComplete callback: gameObject.SetActive(false);
            }
        }

        public void EnableLoginUI()
        {
            gameObject.SetActive(true);
        }

        public void DisableLoginUI()
        {
            gameObject.SetActive(false);
        }

        public void ToggleLobbyCanvas()
        {
            toggleObject.OnToggleObjects();
            PhotonConnector.OnLobbyJoined -= HandleLobbyJoined;
        }
    }
}
