using System;
using DG.Tweening;
using Photon.Pun;
using UnityEngine;
using EverScord.Skill;
using EverScord.Character;
using Photon.Pun.UtilityScripts;

namespace EverScord
{
    public class PortalControl : MonoBehaviour
    {
        [SerializeField] private Collider portalCollider;
        [SerializeField] private Vector3 initialPortalScale;

        private CooldownTimer portalTimer;
        private Coroutine countdownCoroutine;
        private Action onCountdownFinished;

        private int currentCountdownNum = 0;
        private bool isPortalOpened = false;

        void Start()
        {
            GameManager.Instance.InitControl(this);
        }

        void OnTriggerEnter(Collider other)
        {
            if (countdownCoroutine != null)
                return;

            if (((1 << other.gameObject.layer) & GameManager.PlayerLayer) == 0)
                return;

            ActivateCountdown();
        }

        void Update()
        {
            if (countdownCoroutine == null)
                return;

            int previousCountdownNum = currentCountdownNum;
            currentCountdownNum = (int)(portalTimer.Cooldown - portalTimer.ElapsedTime);

            if (previousCountdownNum != currentCountdownNum)
                CharacterControl.CurrentClientCharacter.PlayerUIControl.ChangePortalCountdownNumber(currentCountdownNum);

            if (!portalTimer.IsCooldown)
            {
                StopCoroutine(countdownCoroutine);
                countdownCoroutine = null;

                portalTimer.ResetElapsedTime();
                onCountdownFinished?.Invoke();
                gameObject.SetActive(false);

                CharacterControl.CurrentClientCharacter.PlayerUIControl.HidePortalNotification();
            }
        }

        public void Init(float countdown, Action callback)
        {
            portalTimer = new CooldownTimer(countdown);
            transform.localScale = initialPortalScale;

            currentCountdownNum = (int)countdown + 1;

            onCountdownFinished -= callback;
            onCountdownFinished += callback;
        }

        private void ActivateCountdown()
        {
            SetPortalCollider(false);
            countdownCoroutine = StartCoroutine(portalTimer.RunTimer(true));

            CharacterControl.CurrentClientCharacter.PlayerUIControl.ShowPortalNotification();
        }

        public void TryOpenPortal(float currentProgress)
        {
            if (currentProgress < 1)
                return;

            if (countdownCoroutine != null)
                return;

            if (isPortalOpened)
                return;

            OpenPortal();
        }

        private void OpenPortal()
        {
            isPortalOpened = true;
            gameObject.SetActive(true);

            DOTween.Rewind(ConstStrings.TWEEN_OPEN_PORTAL);
            DOTween.Play(ConstStrings.TWEEN_OPEN_PORTAL);

            // Tween callback: SetPortalCollider(true)

            if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient)
                GameManager.View.RPC(nameof(GameManager.Instance.ReviveAllPlayers), RpcTarget.All);
        }

        public void SetPortalCollider(bool state)
        {
            portalCollider.enabled = state;
        }

        public void SetIsPortalOpened(bool state)
        {
            isPortalOpened = state;
        }
    }
}
