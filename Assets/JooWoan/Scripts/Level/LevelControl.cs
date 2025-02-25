using System;
using UnityEngine;
using DG.Tweening;
using EverScord.Skill;
using EverScord.Character;

namespace EverScord
{
    public class LevelControl : MonoBehaviour
    {
        private const float MAX_PROGRESS = 100f;

        [SerializeField] private GameObject portal;
        [SerializeField] private Collider portalCollider;
        [SerializeField] private Vector3 initialPortalScale;
        [SerializeField] private float increaseAmount;
        [SerializeField] private float countdown;

        private CooldownTimer portalTimer;
        private Coroutine countdownCoroutine;
        private float progress;
        private bool isPortalOpened = false;

        void Awake()
        {
            GameManager.Instance.InitControl(this);

            isPortalOpened = false;
            portal.gameObject.SetActive(false);
            SetPortal(false);

            portalTimer = new CooldownTimer(countdown);
            portal.transform.localScale = initialPortalScale;
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
                
                TeleportPlayers();
            }

            Debug.Log($"Teleport countdown: {countdown - portalTimer.ElapsedTime:F0}");
        }

        void OnTriggerEnter(Collider other)
        {
            if (countdownCoroutine != null)
                return;

            if (((1 << other.gameObject.layer) & GameManager.PlayerLayer) == 0)
                return;

            ActivateCountdown();
            SetPortal(false);
        }

        private void ActivateCountdown()
        {            
            countdownCoroutine = StartCoroutine(portalTimer.RunTimer(true));
        }

        public void IncreaseProgress()
        {
            progress = Mathf.Min(progress + increaseAmount, MAX_PROGRESS);
            TryOpenPortal();

            Debug.Log($"Current Level Progress: {progress}");
        }

        private void TryOpenPortal()
        {
            if (progress < MAX_PROGRESS)
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
            portal.gameObject.SetActive(true);

            DOTween.Rewind(ConstStrings.TWEEN_OPEN_PORTAL);
            DOTween.Play(ConstStrings.TWEEN_OPEN_PORTAL);

            // Tween callback: SetPortal(true)
        }

        public void SetPortal(bool state)
        {
            portalCollider.enabled = state;
        }

        private void TeleportPlayers()
        {
            Debug.Log("Teleporting all players.");

            foreach (var kv in GameManager.Instance.PlayerDict)
            {
                CharacterControl player = kv.Value;

                // play effects

                // make player invisible

                GameManager.LoadNextStage();
            }
        }
    }
}
