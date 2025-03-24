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
        public static Action<bool> OnLobbyToAlteration = delegate { };

        [SerializeField] private UILogin login;
        [SerializeField] private GameObject titleArea, lobbyArea, factorFlashFilter;
        [SerializeField] private DOTweenAnimation titleFadeOutTween;
        [SerializeField] private float showPlayerPanelDelay;
        private bool hasPressedAnything = false;


        void Awake()
        {
            GameManager.Instance.InitControl(this);
            IsExaminingAlteration = false;
            Cursor.visible = false;
        }

        void Start()
        {
            if (!GameManager.IsFirstGameLoad)
            {
                hasPressedAnything = true;
                LevelControl.OnLoadComplete -= ShowLobby;
                LevelControl.OnLoadComplete += ShowLobby;
                return;
            }

            titleArea.SetActive(true);
            lobbyArea.SetActive(false);

            LoadingScreen.ShowScreenFrom1();

            DOTween.Rewind(ConstStrings.TWEEN_SHOW_TITLE);
            DOTween.Play(ConstStrings.TWEEN_SHOW_TITLE);

            SoundManager.Instance.PlayBGM(ConstStrings.BGM_TITLE);
        }

        void Update()
        {
            if (Input.anyKeyDown && !hasPressedAnything)
            {
                Cursor.visible = true;
                hasPressedAnything = true;
                StartCoroutine(ShowLoginUI());
            }
        }

        private IEnumerator ShowLoginUI()
        {
            titleFadeOutTween.DORewind();
            titleFadeOutTween.DOPlay();

            yield return new WaitForSeconds(0.3f);
            login.EnableLoginUI();
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
            SoundManager.Instance.StopBGM(1f);
            LoadingScreen.CoverScreen();
            yield return new WaitForSeconds(2f);

            LoadingScreen.ShowScreen();
            ShowLobby();
        }

        private void ShowLobby()
        {
            LevelControl.OnLoadComplete -= ShowLobby;

            SoundManager.Instance.PlayBGM(ConstStrings.BGM_LOBBY);

            titleArea.SetActive(false);
            lobbyArea.SetActive(true);
            login.ToggleLobbyCanvas();
            OnTransitionToLobby?.Invoke();

            Cursor.visible = true;
            SoundManager.Instance.PlaySound(ConstStrings.SFX_SWOOSH_1);

            DOTween.Rewind(ConstStrings.TWEEN_LOBBYCAM_INTRO);
            DOTween.Play(ConstStrings.TWEEN_LOBBYCAM_INTRO);

            Invoke(nameof(TweenPlayerPanel), showPlayerPanelDelay);
        }

        public void LobbyToAlteration()
        {
            IsExaminingAlteration = true;

            SoundManager.Instance.PlaySound(ConstStrings.SFX_SWOOSH_1);
            SoundManager.Instance.PlaySound(ConstStrings.SFX_ALTERATION_TRANSITION);

            DOTween.Rewind(ConstStrings.TWEEN_LOBBYCAM_INTRO);
            DOTween.Play(ConstStrings.TWEEN_LOBBYCAM_INTRO);

            DOTween.Rewind(ConstStrings.TWEEN_LOBBY2ALTERATION);
            DOTween.Play(ConstStrings.TWEEN_LOBBY2ALTERATION);

            OnLobbyToAlteration?.Invoke(false);

            // callback: Btn_Alteration - UIToggleButton.ToggleObject()
        }

        public void AlterationToLobby()
        {
            IsExaminingAlteration = false;

            SoundManager.Instance.PlaySound(ConstStrings.SFX_SWOOSH_2);

            DOTween.Rewind(ConstStrings.TWEEN_LOBBYCAM_INTRO);
            DOTween.Play(ConstStrings.TWEEN_LOBBYCAM_INTRO);

            DOTween.Rewind(ConstStrings.TWEEN_ALTERATION2LOBBY);
            DOTween.Play(ConstStrings.TWEEN_ALTERATION2LOBBY);

            TweenPlayerPanel();

            OnLobbyToAlteration?.Invoke(true);

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