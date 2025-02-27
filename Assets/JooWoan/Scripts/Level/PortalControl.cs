using System;
using DG.Tweening;
using EverScord.Skill;
using Photon.Pun;
using UnityEngine;

namespace EverScord
{

    public class PortalControl : MonoBehaviour
    {
        [SerializeField] private Collider portalCollider;
        [SerializeField] private Vector3 initialPortalScale;

        private CooldownTimer portalTimer;
        private Coroutine countdownCoroutine;
        private Action onCountdownFinished;

        private bool isPortalOpened = false;

        void OnTriggerEnter(Collider other)
        {
            if (countdownCoroutine != null)
                return;

            if (((1 << other.gameObject.layer) & GameManager.PlayerLayer) == 0)
                return;

            ActivateCountdown();
            SetPortalCollider(false);
        }

        void Update()
        {
            if (countdownCoroutine == null)
                return;

            if (!portalTimer.IsCooldown)
            {
                StopCoroutine(countdownCoroutine);
                countdownCoroutine = null;

                portalTimer.ResetElapsedTime();

                // Teleport players
                onCountdownFinished?.Invoke();
            }

            Debug.Log($"Teleport countdown: {portalTimer.Cooldown - portalTimer.ElapsedTime:F0}");
        }

        private void ActivateCountdown()
        {
            countdownCoroutine = StartCoroutine(portalTimer.RunTimer(true));
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
        }

        public void Init(float countdown, Action callback)
        {
            portalTimer = new CooldownTimer(countdown);
            transform.localScale = initialPortalScale;

            onCountdownFinished -= callback;
            onCountdownFinished += callback;
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
