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
using UnityEngine.VFX;

namespace EverScord
{
    public class PortalControl : MonoBehaviour
    {
        [SerializeField] private VisualEffect warpEffect;
        [SerializeField] private SphereCollider portalCollider;
        [SerializeField] private Vector3 initialPortalScale;
        [SerializeField] private ParticleSystem scanEffect;

        public PhotonView View => photonView;
        public ParticleSystem ScanEffect => scanEffect;

        private PhotonView photonView;
        private CooldownTimer portalTimer;
        private Coroutine countdownCoroutine;
        private Action onCountdownFinished;
        private GameObject teleportEffect;
        private UIMarker uiMarker;

        private int currentCountdownNum;
        private bool isPortalOpened = false;

        public Vector3 ColliderStartPos
        {
            get { return transform.TransformPoint(portalCollider.center); }
        }

        private float colliderWorldRadius;

        public static Action OnLevelClear = delegate { };
        public static Action OnNextStage = delegate { };

        private void Awake()
        {
            photonView = GetComponent<PhotonView>();
            uiMarker = gameObject.AddComponent<UIMarker>();
            uiMarker.Initialize(PointMarkData.EType.Portal);
            scanEffect.gameObject.SetActive(false);
        }

        void Start()
        {
            teleportEffect = ResourceManager.Instance.GetAsset<GameObject>(AssetReferenceManager.TeleportEffect_ID);
        }

        private void OnEnable()
        {
            uiMarker.SetActivate(true);
            uiMarker.UpdatePosition(transform.position);
        }

        void OnDisable()
        {
            ResetPortal();
            uiMarker.SetActivate(false);
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
            GameManager.Instance.InitControl(this);
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
            scanEffect.gameObject.SetActive(true);
            countdownCoroutine = StartCoroutine(portalTimer.RunTimer(true));

            CharacterControl.CurrentClientCharacter.PlayerUIControl.ShowPortalNotification();
            OnNextStage?.Invoke();
        }

        public void TryOpenPortal(float currentProgress)
        {
            if (currentProgress < 1)
                return;

            if (countdownCoroutine != null)
                return;

            if (isPortalOpened)
                return;

            if (GameManager.CurrentLevelIndex == GameManager.Instance.LevelController.MaxLevelIndex)
            {
                if (!LevelControl.IsBossMode)
                    GameManager.Instance.LevelController.SetBossMode(true);
                return;
            }

            OpenPortal();
        }

        private void OpenPortal()
        {
            isPortalOpened = true;
            gameObject.SetActive(true);

            SoundManager.Instance.PlaySound(ConstStrings.SFX_PORTAL_START);

            GameManager.Instance.AugmentControl.ShowAugmentCards();
            OnLevelClear?.Invoke();

            DOTween.Rewind(ConstStrings.TWEEN_OPEN_PORTAL);
            DOTween.Play(ConstStrings.TWEEN_OPEN_PORTAL);

            if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient)
                GameManager.View.RPC(nameof(GameManager.Instance.ReviveAllPlayers), RpcTarget.All);
        }

        public void ClosePortal()
        {
            DOTween.Rewind(ConstStrings.TWEEN_CLOSE_PORTAL);
            DOTween.Play(ConstStrings.TWEEN_CLOSE_PORTAL);

            // Tween callback: gameObject.SetActive(false)
        }

        public void ResetPortal()
        {
            SetActive(false);
            SetIsPortalOpened(false);
            SetPortalCollider(false);

            transform.localScale = initialPortalScale;
            currentCountdownNum = (int)portalTimer.Cooldown + 1;
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
            return Physics.OverlapSphere(ColliderStartPos, colliderWorldRadius, GameManager.PlayerLayer);
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

            bool flag = false;

            for (int i = 0; i < targetPlayers.Count; i++)
            {
                flag = true;

                var effect = Instantiate(teleportEffect, CharacterSkill.SkillRoot);
                effect.transform.position = targetPlayers[i].PlayerTransform.position;

                targetPlayers[i].Teleport(GetRandomPosition());
                StartCoroutine(DelayTeleportEffect(targetPlayers[i], 0.2f));
            }

            if (flag)
                SoundManager.Instance.PlaySound(ConstStrings.SFX_TELEPORT);
        }

        private IEnumerator DelayTeleportEffect(CharacterControl player, float delay)
        {
            yield return new WaitForSeconds(delay);

            player.BlinkEffects.ChangeBlinkTemporarily(GameManager.InvincibleBlinkInfo);
            player.BlinkEffects.LoopBlink(true);

            var effect = Instantiate(teleportEffect, CharacterSkill.SkillRoot);
            effect.transform.position = player.PlayerTransform.position;
        }

        public void PlayWarpEffect(bool isExit)
        {
            if (isExit)
                warpEffect.SendEvent(ConstStrings.VFX_WARP_OUT);
            else
                warpEffect.SendEvent(ConstStrings.VFX_WARP_IN);
        }

        public Vector3 GetRandomPosition()
        {
            Vector3 randomPoint = UnityEngine.Random.insideUnitCircle;
            randomPoint = ColliderStartPos + randomPoint * colliderWorldRadius * 0.6f;
            randomPoint.y = CharacterControl.CurrentClientCharacter.PlayerTransform.position.y;

            return randomPoint;
        }

        public void SetActive(bool state)
        {
            gameObject.SetActive(state);
        }

        public void MovePosition(Vector3 position)
        {
            transform.position = position;
        }

        [PunRPC]
        public void SyncSetPortal(bool state)
        {
            SetPortalCollider(state);
        }
    }
}
