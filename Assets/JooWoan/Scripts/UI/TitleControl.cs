using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace EverScord
{
    public class TitleControl : MonoBehaviour
    {
        public bool IsExaminingAlteration { get; private set; }
        public static Action OnTransitionToLobby = delegate { };

        [SerializeField] private UILogin login;
        [SerializeField] private GameObject titleArea, lobbyArea;
        [SerializeField] private DOTweenAnimation titleFadeOutTween;
        private bool hasPressedAnything = false;

        void Awake()
        {
            GameManager.Instance.InitControl(this);
            titleArea.SetActive(true);
            lobbyArea.SetActive(false);
            IsExaminingAlteration = false;
        }

        void Start()
        {
            login.DisableLoginUI();
            LoadingScreen.ShowScreenFrom1();
            DOTween.PlayForward(ConstStrings.TWEEN_SHOW_TITLE);
            GameManager.SetIsFirstGameLoad(false);
        }

        void Update()
        {
            if (Input.anyKeyDown && !hasPressedAnything)
            {
                hasPressedAnything = true;
                StartCoroutine(ShowLoginUI());
            }
        }

        private IEnumerator ShowLoginUI()
        {
            titleFadeOutTween.DORewind();
            titleFadeOutTween.DOPlay();

            yield return new WaitForSeconds(0.3f);
            login.ShowLoginUI(true);
        }

        public bool TransitionLobby()
        {
            StartCoroutine(StartTransitionLobby());
            return true;
        }

        public IEnumerator StartTransitionLobby()
        {
            LoadingScreen.CoverScreen();
            yield return new WaitForSeconds(2f);

            login.ToggleLobbyCanvas();
            LoadingScreen.ShowScreen();

            ShowLobby();
        }

        private void ShowLobby()
        {
            titleArea.SetActive(false);
            lobbyArea.SetActive(true);
            OnTransitionToLobby?.Invoke();
            DOTween.Rewind(ConstStrings.TWEEN_LOBBYCAM_INTRO);
            DOTween.Play(ConstStrings.TWEEN_LOBBYCAM_INTRO);
        }

        public void LobbyToAlteration()
        {
            IsExaminingAlteration = true;

            DOTween.Rewind(ConstStrings.TWEEN_LOBBY2ALTERATION);
            DOTween.Play(ConstStrings.TWEEN_LOBBY2ALTERATION);

            // callback: Btn_Alteration - UIToggleButton.ToggleObject()
        }

        public void AlterationToLobby()
        {
            IsExaminingAlteration = false;

            DOTween.Rewind(ConstStrings.TWEEN_ALTERATION2LOBBY);
            DOTween.Play(ConstStrings.TWEEN_ALTERATION2LOBBY);

            // callback: AlterationPanel - ReturnButton - UIToggleButton.ToggleObject()
        }
    }
}