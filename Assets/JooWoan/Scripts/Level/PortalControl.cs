using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;
using DG.Tweening;
using Photon.Pun;
using UnityEngine;
using EverScord.Skill;
using EverScord.Character;
using EverScord.Effects;

namespace EverScord
{
    public class PortalControl : MonoBehaviour
    {
        [SerializeField] private SphereCollider portalCollider;
        [SerializeField] private Vector3 initialPortalScale;

        private CooldownTimer portalTimer;
        private Coroutine countdownCoroutine;
        private Action onCountdownFinished;
        private GameObject teleportEffect;

        private int currentCountdownNum = 0;
        private bool isPortalOpened = false;

        private Vector3 colliderStartPos;
        private float colliderWorldRadius;

        void Start()
        {
            teleportEffect = ResourceManager.Instance.GetAsset<GameObject>(AssetReferenceManager.TeleportEffect_ID);
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
                BringPlayersOutofRange();
                onCountdownFinished?.Invoke();

                CharacterControl.CurrentClientCharacter.PlayerUIControl.HidePortalNotification();
            }
        }

        public void Init(float countdown, Action callback)
        {
            colliderStartPos = transform.TransformPoint(portalCollider.center);
            colliderWorldRadius = portalCollider.radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z);

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

        public Collider[] CheckPlayersInRange()
        {
            return Physics.OverlapSphere(colliderStartPos, colliderWorldRadius, GameManager.PlayerLayer);
        }

        private void BringPlayersOutofRange()
        {
            Collider[] hits = CheckPlayersInRange();
            List<CharacterControl> targetPlayers = GameManager.Instance.PlayerDict.Values.ToList();

            for (int i = targetPlayers.Count - 1; i >= 0; i--)
            {
                for (int j = 0; j < hits.Length; j++)
                {
                    if (targetPlayers[i].gameObject == hits[j].gameObject)
                    {
                        targetPlayers.RemoveAt(i);
                        break;
                    }
                }
            }

            for (int i = 0; i < targetPlayers.Count; i++)
            {
                var effect = Instantiate(teleportEffect, CharacterSkill.SkillRoot);
                effect.transform.position = targetPlayers[i].PlayerTransform.position;

                targetPlayers[i].Teleport(GetRandomPosition());
                StartCoroutine(DelayTeleportEffect(targetPlayers[i], 0.2f));
            }
        }

        private IEnumerator DelayTeleportEffect(CharacterControl player, float delay)
        {
            yield return new WaitForSeconds(delay);

            player.BlinkEffects.ChangeBlinkTemporarily(GameManager.InvincibleBlinkInfo);
            player.BlinkEffects.LoopBlink(true);

            var effect = Instantiate(teleportEffect, CharacterSkill.SkillRoot);
            effect.transform.position = player.PlayerTransform.position;
        }

        private Vector3 GetRandomPosition()
        {
            Vector3 randomPoint = UnityEngine.Random.insideUnitCircle;
            randomPoint = colliderStartPos + randomPoint * colliderWorldRadius;
            randomPoint.y = CharacterControl.CurrentClientCharacter.PlayerTransform.position.y;

            return randomPoint;
        }

        public void SetActive(bool state)
        {
            gameObject.SetActive(state);
        }
    }
}
