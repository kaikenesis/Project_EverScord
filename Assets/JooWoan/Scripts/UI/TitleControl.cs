using System;
using System.Collections;
using DG.Tweening;
using EverScord.UI;
using UnityEngine;

namespace EverScord
{
    public class TitleControl : MonoBehaviour
    {
        public bool IsExaminingAlteration { get; private set; }
        public static Action OnTransitionToLobby = delegate { };

        [SerializeField] private UILogin login;
        [SerializeField] private GameObject titleArea, lobbyArea, factorFlashFilter;
        [SerializeField] private DOTweenAnimation titleFadeOutTween, alterationBgTween;
        [SerializeField] private float showPlayerPanelDelay;
        private bool hasPressedAnything = false;

        void Awake()
        {
            if (!GameManager.IsFirstGameLoad)
            {
                hasPressedAnything = true;
                LevelControl.OnLoadComplete -= ShowLobby;
                LevelControl.OnLoadComplete += ShowLobby;
                return;
            }

            GameManager.Instance.InitControl(this);
            IsExaminingAlteration = false;

            titleArea.SetActive(true);
            lobbyArea.SetActive(false);

            login.DisableLoginUI();
            LoadingScreen.ShowScreenFrom1();
            DOTween.PlayForward(ConstStrings.TWEEN_SHOW_TITLE);
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
            if (!GameManager.IsFirstGameLoad)
                return false;

            GameManager.SetIsFirstGameLoad(false);
            StartCoroutine(StartTransitionLobby());
            return true;
        }

        public IEnumerator StartTransitionLobby()
        {
            LoadingScreen.CoverScreen();
            yield return new WaitForSeconds(2f);

            LoadingScreen.ShowScreen();
            ShowLobby();
        }

        private void ShowLobby()
        {
            LevelControl.OnLoadComplete -= ShowLobby;

            titleArea.SetActive(false);
            lobbyArea.SetActive(true);
            login.ToggleLobbyCanvas();
            OnTransitionToLobby?.Invoke();

            DOTween.Rewind(ConstStrings.TWEEN_LOBBYCAM_INTRO);
            DOTween.Play(ConstStrings.TWEEN_LOBBYCAM_INTRO);
            Invoke(nameof(TweenPlayerPanel), showPlayerPanelDelay);
        }

        public void LobbyToAlteration()
        {
            IsExaminingAlteration = true;

            DOTween.Rewind(ConstStrings.TWEEN_LOBBYCAM_INTRO);
            DOTween.Play(ConstStrings.TWEEN_LOBBYCAM_INTRO);

            DOTween.Rewind(ConstStrings.TWEEN_LOBBY2ALTERATION);
            DOTween.Play(ConstStrings.TWEEN_LOBBY2ALTERATION);

            // callback: Btn_Alteration - UIToggleButton.ToggleObject()
        }

        public void AlterationToLobby()
        {
            IsExaminingAlteration = false;

            DOTween.Rewind(ConstStrings.TWEEN_LOBBYCAM_INTRO);
            DOTween.Play(ConstStrings.TWEEN_LOBBYCAM_INTRO);

            DOTween.Rewind(ConstStrings.TWEEN_ALTERATION2LOBBY);
            DOTween.Play(ConstStrings.TWEEN_ALTERATION2LOBBY);

            TweenPlayerPanel();

            // callback: AlterationPanel - ReturnButton - UIToggleButton.ToggleObject()
        }

        public void TweenAlterationPanel(bool isTransitionToAlteration)
        {
            if (isTransitionToAlteration)
            {
                DOTween.PlayForward(ConstStrings.TWEEN_ALTERATION);
            }
            else
                DOTween.PlayBackwards(ConstStrings.TWEEN_ALTERATION);
        }

        public void TweenPlayerPanel()
        {
            DOTween.Rewind(ConstStrings.TWEEN_PLAYERPANEL);
            DOTween.Play(ConstStrings.TWEEN_PLAYERPANEL);
        }

        public void DarkenAlterationBackground(bool state)
        {
            if (state)
                DOTween.PlayForward(ConstStrings.TWEEN_DARKEN_ALTERATION_BG);
            else
                DOTween.PlayBackwards(ConstStrings.TWEEN_DARKEN_ALTERATION_BG);
        }

        public void AlterationAppliedTween()
        {
            SoundManager.Instance.PlaySound(ConstStrings.SFX_ALTERATION_APPLIED, 0.6f);

            factorFlashFilter.SetActive(true);
            DOTween.Rewind(ConstStrings.TWEEN_ALTERATION_APPLIED);
            DOTween.Play(ConstStrings.TWEEN_ALTERATION_APPLIED);
        }
    }
}