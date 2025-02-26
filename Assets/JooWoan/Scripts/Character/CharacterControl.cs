using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;
using Photon.Pun;
using EverScord.Weapons;
using EverScord.UI;
using EverScord.GameCamera;
using EverScord.Skill;
using EverScord.Effects;

namespace EverScord.Character
{
    public class CharacterControl : MonoBehaviour, IPunInstantiateMagicCallback, IPunObservable
    {
        private const float SKIN_RATIO = 0.1f;
        private const float REMOTE_LERP_VALUE = 10f;
        private const float BODY_CENTER = 2f;
        private const float BODY_ROTATESTART_ANGLE = 2f;

        [Header("Character")]
        [SerializeField] private float speed;
        [SerializeField] private float gravity;
        [SerializeField] private float mass;
        [SerializeField] private float bodyRotateSpeed;
        [SerializeField] private float maxHealth;
        [SerializeField] private float currentHealth;

        [Header("Ground Check")]
        [SerializeField] private float groundCheckRadius;
        [SerializeField] private Vector3 groundCheckOffset;

        [Header("Weapon")]
        [SerializeField] private Weapon weapon;

        [Header("Skill")]
        [SerializeField] private List<SkillActionInfo> skillList;

        [Header("UI")]
        [SerializeField] private PlayerUI uiPrefab;

        [Header("Camera")]
        [SerializeField] private CharacterCamera cameraPrefab;

        [Header("Rig")]
        [SerializeField] private CharacterRigControl rigLayerPrefab;

        public PlayerUI PlayerUIControl                                 { get; private set; }
        public CharacterRigControl RigControl                           { get; private set; }
        public CharacterAnimation AnimationControl                      { get; private set; }
        public CharacterPhysics PhysicsControl                          { get; private set; }
        public CharacterCamera CameraControl                            { get; private set; }
        public Transform PlayerTransform                                { get; private set; }
        public SkinnedMeshRenderer[] SkinRenderers                      { get; private set; }
        public LayerMask OriginalSkinLayer                              { get; private set; }
        public Vector3 MouseRayHitPos                                   { get; private set; }
        public Vector3 MoveVelocity                                     { get; private set; }
        public SkillAction CurrentSkillAction                           { get; private set; }
        public PlayerData.EJob CharacterJob                             { get; private set; }
        public CharState State                                          { get; private set; }

        private InputInfo playerInputInfo = new InputInfo();
        public InputInfo PlayerInputInfo => playerInputInfo;
        public Weapon PlayerWeapon => weapon;
        public PhotonView CharacterPhotonView => photonView;
        public CharacterController Controller => controller;
        public Vector3 LookDir => lookDir;
        public float CharacterSpeed => speed;

        private static GameObject deathEffect, reviveEffect, beamEffect;
        private static ParticleSystem hitEffect1, hitEffect2;
        private ParticleSystem healEffect;
        private BlinkEffect blinkEffect;
        private PhotonView photonView;
        private CharacterController controller;
        private Vector3 movement, lookDir, moveInput, moveDir;
        private Vector3 remoteMouseRayHitPos;
        private Action onDecreaseHealth;
        private LayerMask groundAndEnemyLayer;

        public float CurrentHealth
        {
            get { return currentHealth; }
            set
            {
                bool previousIsLowHealth = IsLowHealth;

                currentHealth = value;

                bool afterIsLowHealth = IsLowHealth;

                // Change Health UI

                if (!IsDead && currentHealth <= 0)
                    StartCoroutine(HandleDeath());
                
                if (previousIsLowHealth != afterIsLowHealth)
                    blinkEffect.LoopBlink(IsLowHealth);

                if (photonView.IsMine)
                {
                    PlayerUIControl.SetGrayscaleScreen(currentHealth);
                    PlayerUIControl.SetBloodyScreen(currentHealth, afterIsLowHealth);
                }
            }
        }

        void Awake()
        {
            PlayerUI.SetUIRoot();
            CharacterCamera.SetCameraRoot();

            photonView       = GetComponent<PhotonView>();

            PlayerTransform  = transform;
            PhysicsControl   = new CharacterPhysics(gravity, mass);

            controller       = GetComponent<CharacterController>();
            AnimationControl = GetComponent<CharacterAnimation>();
            SkinRenderers    = GetComponentsInChildren<SkinnedMeshRenderer>();
            
            PlayerUIControl  = Instantiate(uiPrefab, PlayerUI.Root);
            CameraControl    = Instantiate(cameraPrefab, CharacterCamera.Root);

            // Unity docs: Set skinwidth 10% of the Radius
            controller.skinWidth = controller.radius * SKIN_RATIO;
            
            if (SkinRenderers.Length > 0)
                OriginalSkinLayer = 1 << SkinRenderers[0].gameObject.layer;

            currentHealth = maxHealth;

            AnimationControl.Init(photonView);
            weapon.Init(this);

            RigControl = Instantiate(rigLayerPrefab, AnimationControl.Anim.transform);
            RigControl.Init(AnimationControl.Anim.transform, GetComponent<Animator>(), weapon);
            RigControl.SetAimWeight(false);

            if (!photonView.IsMine)
            {
                PlayerUIControl.gameObject.SetActive(false);
                CameraControl.gameObject.SetActive(false);
            }
            else
                PlayerUIControl.Init();

            CameraControl.Init(PlayerTransform, photonView.IsMine);

            blinkEffect = BlinkEffect.Create(transform, GameManager.HurtBlinkInfo);
            groundAndEnemyLayer = GameManager.GroundLayer | GameManager.EnemyLayer;

            CharacterJob = GameManager.Instance.PlayerData.job;

            DontDestroyOnLoad(gameObject);
        }

        void Start()
        {
            SetJobAndSkills();
            SetReviveCircle();
            SetEffects();
        }

        void Update()
        {
            if (photonView.IsMine && Input.GetKeyDown(KeyCode.F1))
                IncreaseHP(10);

            if (!photonView.IsMine)
            {
                LerpRemoteInfo();
                return;
            }

            SetInput();

            if (IsDead)
                return;

            SetMovingDirection();

            PhysicsControl.ApplyGravity(this);
            PhysicsControl.ApplyImpact(this);

            Move();
            AnimationControl.AnimateMovement(this, moveDir);

            TrackAim();
            RotateBody();

            weapon.TryReload(this);
            weapon.Shoot(this);

            UseSkills();
        }

        private void LerpRemoteInfo()
        {
            MouseRayHitPos = Vector3.Lerp(MouseRayHitPos, remoteMouseRayHitPos, Time.deltaTime * REMOTE_LERP_VALUE);
        }

        private void SetInput()
        {
            playerInputInfo = InputControl.ReceiveInput();
            playerInputInfo = InputControl.GetCameraRelativeInput(playerInputInfo, CameraControl.Cam);

            moveInput = playerInputInfo.cameraRelativeInput;
        }

        private void SetMovingDirection()
        {
            if (PhysicsControl.IsImpactAdded)
            {
                moveDir = Quaternion.Inverse(transform.rotation) * PhysicsControl.ImpactVelocity.normalized;
                return;
            }

            if (moveInput.magnitude > 1f)
                moveInput.Normalize();

            moveDir = PlayerTransform.InverseTransformDirection(moveInput);
        }

        private void Move()
        {
            if (HasState(CharState.SKILL_STANCE))
                return;

            movement = new Vector3(moveInput.x, 0, moveInput.z);

            Vector3 velocity = movement * speed;
            velocity.y = PhysicsControl.FallSpeed;

            if (PhysicsControl.IsImpactAdded)
                velocity = PhysicsControl.ImpactVelocity.normalized * speed;
            
            controller.Move(velocity * Time.deltaTime);
        }

        private bool ProcessMouseRaycast(out Ray ray, out RaycastHit hit)
        {
            ray = CameraControl.Cam.ScreenPointToRay(playerInputInfo.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity, groundAndEnemyLayer);

            hit = default;
            bool groundFlag = false;
            bool enemyFlag = false;

            for (int i = 0; i < hits.Length; i++)
            {
                LayerMask hitLayerMask = 1 << hits[i].collider.gameObject.layer;

                if ((GameManager.EnemyLayer & hitLayerMask) != 0)
                {
                    IEnemy enemy = hits[i].transform.GetComponent<IEnemy>();
                    OutlineControl.EnableEnemyOutline(photonView, enemy);
                    enemyFlag = true;
                }
                else if ((GameManager.GroundLayer & hitLayerMask) != 0)
                {
                    hit = hits[i];
                    groundFlag = true;
                    break;
                }
            }

            if (!enemyFlag)
                OutlineControl.DisableEnemyOutline(photonView);

            return groundFlag;
        }

        public void TrackAim()
        {
            if (HasState(CharState.RIGID_ANIMATING))
                return;
            
            if (!ProcessMouseRaycast(out Ray ray, out RaycastHit hit))
                return;

            MouseRayHitPos = hit.point;

            Vector3 lookPosition = new Vector3(
                hit.point.x,
                PlayerTransform.position.y,
                hit.point.z
            );

            lookDir = (lookPosition - PlayerTransform.position).normalized;

            Vector3 cam2GunPointDir = weapon.GunPoint.position - ray.origin;

            if (!Physics.Raycast(ray.origin, cam2GunPointDir, out RaycastHit cam2GunPointRayHit, Mathf.Infinity, GameManager.GroundLayer))
                return;

            Vector3 gunPoint2MouseDir = hit.point - cam2GunPointRayHit.point;

            float player2Mouse = Vector3.Distance(transform.position, MouseRayHitPos);
            float player2Gunpoint = Vector3.Distance(transform.position, cam2GunPointRayHit.point);

            if (player2Mouse < player2Gunpoint)
                gunPoint2MouseDir = MouseRayHitPos - transform.position;

            weapon.SetGunPointDirection(gunPoint2MouseDir);

            Vector3 aimPosition = weapon.GunPoint.position + weapon.GunPoint.forward * weapon.WeaponRange;
            weapon.AimPoint.position = aimPosition;
        }

        private void RotateBody()
        {
            if (HasState(CharState.RIGID_ANIMATING))
                return;
            
            float angle = Vector3.Angle(lookDir, PlayerTransform.forward);

            if (angle > BODY_ROTATESTART_ANGLE)
            {
                AnimationControl.Rotate(!IsMoving);
                Quaternion lookRotation = Quaternion.LookRotation(lookDir);

                PlayerTransform.rotation = Quaternion.RotateTowards(
                    PlayerTransform.rotation,
                    lookRotation,
                    bodyRotateSpeed * Time.deltaTime
                );
                return;
            }

            AnimationControl.Rotate(false);
        }

        private void UseSkills()
        {
            for (int i = 0; i < skillList.Count; i++)
            {
                SkillActionInfo info = skillList[i];

                if (!playerInputInfo.PressedSkill(i))
                    continue;

                if (weapon.IsReloading && !skillList[i].SkillAction.CanAttackWhileSkill)
                    continue;

                if (IsUsingSkill && CurrentSkillAction != skillList[i].SkillAction)
                    continue;

                info.SkillAction.Activate();
                CurrentSkillAction = skillList[i].SkillAction;

                if (PhotonNetwork.IsConnected)
                    photonView.RPC(nameof(SyncUseSkill), RpcTarget.Others, i);
                
                break;
            }
        }

        private void SetJobAndSkills()
        {
            if (!photonView.IsMine)
                return;

            for (int i = 0; i < skillList.Count; i++)
            {
                skillList[i].Init(this, i, CharacterJob);

                if (PhotonNetwork.IsConnected)
                    photonView.RPC(nameof(SyncJobAndSkills), RpcTarget.Others, i, (int)CharacterJob);
            }
        }

        private void SetReviveCircle()
        {
            foreach (var kv in GameManager.Instance.PlayerDict)
            {
                CharacterControl player = kv.Value;
                player.PlayerUIControl.InitReviveCircle(player.PlayerTransform, player.CharacterPhotonView.ViewID);
            }
        }

        private void SetEffects()
        {
            deathEffect  = ResourceManager.Instance.GetAsset<GameObject>(AssetReferenceManager.DeathEffect_ID);
            reviveEffect = ResourceManager.Instance.GetAsset<GameObject>(AssetReferenceManager.ReviveEffect_ID);
            beamEffect   = ResourceManager.Instance.GetAsset<GameObject>(AssetReferenceManager.ReviveBeam_ID);

            if (!hitEffect1)
            {
                var effect1 = ResourceManager.Instance.GetAsset<GameObject>(AssetReferenceManager.HitEffect1_ID);
                var effect2 = ResourceManager.Instance.GetAsset<GameObject>(AssetReferenceManager.HitEffect2_ID);

                hitEffect1 = Instantiate(effect1, CharacterSkill.SkillRoot).GetComponent<ParticleSystem>();
                hitEffect2 = Instantiate(effect2, CharacterSkill.SkillRoot).GetComponent<ParticleSystem>();

                hitEffect1.gameObject.SetActive(false);
                hitEffect2.gameObject.SetActive(false);
            }

            var effect3 = ResourceManager.Instance.GetAsset<GameObject>(AssetReferenceManager.HealEffect_ID);
            healEffect = Instantiate(effect3, PlayerTransform).GetComponent<ParticleSystem>();
            healEffect.gameObject.SetActive(false);
        }

        public void SetSpeed(float speed)
        {
            this.speed = speed;
        }

        public void SetIsAiming(bool state)
        {
            IsAiming = state;
        }

        public void SetMouseButtonDown(bool state)
        {
            playerInputInfo.pressedLeftMouseButton = state;
        }

        public void SetPosition(Vector3 position)
        {
            transform.position = position;
        }

        public void SetState(SetCharState mode, CharState state)
        {
            switch (mode)
            {
                case SetCharState.ADD:
                    State |= state;
                    break;

                case SetCharState.REMOVE:
                    State &= ~state;
                    break;

                case SetCharState.CLEAR:
                    State = 0;
                    break;

                default:
                    break;
            }
        }

        public bool HasState(CharState state)
        {
            return (State & state) != 0;
        }

        public void SubscribeOnDecreaseHealth(Action subscriber)
        {
            onDecreaseHealth -= subscriber;
            onDecreaseHealth += subscriber;
        }

        public void UnsubscribeOnDecreaseHealth(Action subscriber)
        {
            onDecreaseHealth -= subscriber;
        }

        public void DecreaseHP(float amount)
        {
            bool isInvincible = HasState(CharState.INVINCIBLE);

            if (isInvincible)
                blinkEffect.ChangeBlinkTemporarily(GameManager.InvincibleBlinkInfo);
            
            blinkEffect.Blink();
            PlayHitEffects();

            if (PhotonNetwork.IsConnected)
                photonView.RPC(nameof(SyncHitEffects), RpcTarget.Others, false, isInvincible);

            if (IsDead)
                return;

            if (!isInvincible)
                CurrentHealth = Mathf.Max(0, CurrentHealth - amount);

            onDecreaseHealth?.Invoke();

            if (PhotonNetwork.IsConnected)
                photonView.RPC(nameof(SyncHealth), RpcTarget.Others, currentHealth, false);
        }

        public void IncreaseHP(float amount, bool isExternalHeal = false)
        {
            if (IsDead)
                return;
            
            PlayHealEffects();

            CurrentHealth = Mathf.Min(maxHealth, CurrentHealth + amount);

            if (PhotonNetwork.IsConnected && (photonView.IsMine || isExternalHeal))
            {
                photonView.RPC(nameof(SyncHitEffects), RpcTarget.Others, true, false);
                photonView.RPC(nameof(SyncHealth), RpcTarget.Others, currentHealth, true);
            }
        }

        private IEnumerator HandleDeath()
        {
            SetState(SetCharState.ADD, CharState.DEATH);

            Instantiate(deathEffect, transform.position, Quaternion.identity);
            blinkEffect.LoopBlink(false);
            
            RigControl.SetAimWeight(false);
            RigControl.SetMainRigWeight(false);

            AnimationControl.SetUpperMask(false, true);
            AnimationControl.Play(AnimationControl.AnimInfo.Death);

            OutlineControl.SetCharacterOutline(this, true);

            yield return new WaitForSeconds(AnimationControl.AnimInfo.Death.length);

            PlayerUIControl.SetReviveCircle(true);
            
            if (PhotonNetwork.IsConnected && photonView.IsMine)
                photonView.RPC(nameof(SyncReviveCircle), RpcTarget.Others, true);
        }

        public IEnumerator HandleRevival()
        {
            SetState(SetCharState.ADD, CharState.INVINCIBLE);

            Instantiate(reviveEffect, PlayerTransform.position, Quaternion.identity);
            Instantiate(beamEffect, PlayerTransform.position, Quaternion.identity);

            PlayHealEffects();

            CurrentHealth = maxHealth;

            AnimationControl.Play(AnimationControl.AnimInfo.Revive);
            PlayerUIControl.SetReviveCircle(false);

            OutlineControl.SetCharacterOutline(this, false);
            
            yield return new WaitForSeconds(AnimationControl.AnimInfo.Revive.length);

            RigControl.SetMainRigWeight(true);
            AnimationControl.CrossFade(new AnimationParam(AnimationControl.AnimInfo.Idle.name, 0.25f));

            for (float t = 0; t < 0.5f; t += Time.deltaTime)
                yield return null;

            SetState(SetCharState.REMOVE, CharState.INVINCIBLE);
            SetState(SetCharState.REMOVE, CharState.DEATH);
        }

        private void PlayHitEffects()
        {
            Vector3 hitPos = new Vector3(PlayerTransform.position.x, BODY_CENTER, PlayerTransform.position.z);

            hitEffect1.transform.position = hitPos;
            hitEffect2.transform.position = hitPos;

            if (!hitEffect1.gameObject.activeSelf)
                hitEffect1.gameObject.SetActive(true);

            if (!hitEffect2.gameObject.activeSelf)
                hitEffect2.gameObject.SetActive(true);

            hitEffect1.Emit(1);
            hitEffect2.Emit(1);
        }

        private void PlayHealEffects()
        {
            healEffect.transform.position = PlayerTransform.position;

            if (!healEffect.gameObject.activeSelf)
                healEffect.gameObject.SetActive(true);
            
            healEffect.Emit(1);
        }

        public bool IsGrounded
        {
            get
            {
                return Physics.CheckSphere(
                    PlayerTransform.TransformPoint(groundCheckOffset),
                    groundCheckRadius,
                    GameManager.GroundLayer
                );
            }
        }

        public bool IsUsingSkill
        {
            get
            {
                foreach (var info in skillList)
                {
                    if (info.SkillAction.IsUsingSkill)
                        return true;
                }

                return false;
            }
        }

        public bool IsLowHealth
        {
            get { return currentHealth <= maxHealth * 0.1f; }
        }

        public bool IsDead
        {
            get { return HasState(CharState.DEATH); }
        }

        public bool IsAiming { get; private set; }
        public bool IsShooting => playerInputInfo.holdLeftMouseButton;
        public bool IsMoving => moveInput.magnitude > 0 || PhysicsControl.IsImpactAdded;

        #region Photon
        ////////////////////////////////////////  PUN RPC  //////////////////////////////////////////////////////

        public void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            GameManager.Instance.AddPlayerPhotonView(info.photonView);
            GameManager.Instance.InitControl(this);
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(MouseRayHitPos);
            }
            else if (stream.IsReading)
            {
                remoteMouseRayHitPos = (Vector3)stream.ReceiveNext();
            }
        }

        [PunRPC]
        private void SyncJobAndSkills(int index, int characterJob)
        {
            CharacterJob = (PlayerData.EJob)characterJob;
            skillList[index].Init(this, index, (PlayerData.EJob)characterJob);
        }

        [PunRPC]
        private void SyncUseSkill(int index)
        {
            CurrentSkillAction = skillList[index].SkillAction;
            skillList[index].SkillAction.Activate();
        }

        [PunRPC]
        public void SyncCounterSkill(Vector3 mouseRayHitPos, bool toggle, int index)
        {
            MouseRayHitPos = MouseRayHitPos;
            playerInputInfo.pressedLeftMouseButton = true;
        }

        [PunRPC]
        public void SyncCounterSupport(int viewID, int index)
        {
            CounterSkillAction skillAction = (CounterSkillAction)skillList[index].SkillAction;
            skillAction.SyncGrantBuff(viewID);

            playerInputInfo.pressedLeftMouseButton = true;
        }

        [PunRPC]
        public void SyncThrowSkill(Vector3 mouseRayHitPos, Vector3 thrownPosition, Vector3 groundDirection, float initialVelocity, float trajectoryAngle, float estimatedTime, int index)
        {
            ThrowSkillAction skillAction = (ThrowSkillAction)skillList[index].SkillAction;

            MouseRayHitPos = mouseRayHitPos;
            playerInputInfo.pressedLeftMouseButton = true;

            TrajectoryInfo info = new TrajectoryInfo()
            {
                ThrownPosition  = thrownPosition,
                GroundDirection = groundDirection,
                InitialVelocity = initialVelocity,
                TrajectoryAngle = trajectoryAngle,
                EstimatedTime   = estimatedTime
            };

            skillAction.Predictor.SyncInfo(info);
            skillAction.StartCoroutine(skillAction.ThrowObject(info));
        }

        [PunRPC]
        public void SyncJumpAttackSkill(Vector3 mouseRayHitPos, Vector3 landingPosition, int index)
        {
            JumpAttackAction skillAction = (JumpAttackAction)skillList[index].SkillAction;

            MouseRayHitPos = mouseRayHitPos;
            playerInputInfo.pressedLeftMouseButton = true;

            skillAction.SyncInfo(mouseRayHitPos, landingPosition);
        }

        [PunRPC]
        private void SyncHealth(float health, bool isIncreasing)
        {
            CurrentHealth = health;

            if (!isIncreasing)
                onDecreaseHealth?.Invoke();
        }

        [PunRPC]
        private void SyncHitEffects(bool isIncreasing, bool isInvincible)
        {
            if (isIncreasing)
                PlayHealEffects();
            else
            {
                if (isInvincible)
                    blinkEffect.ChangeBlinkTemporarily(GameManager.InvincibleBlinkInfo);

                blinkEffect.Blink();
                PlayHitEffects();
            }
        }

        [PunRPC]
        private void SyncReviveCircle(bool state)
        {
            PlayerUIControl.SetReviveCircle(state);
        }

        
        [PunRPC]
        public void SyncExitCircle()
        {
            PlayerUIControl.ReviveCircleControl.SyncExitCircle();
        }

        ////////////////////////////////////////  PUN RPC  //////////////////////////////////////////////////////
        #endregion

        #region GIZMOS
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0, 1, 0, 0.4f);

            Gizmos.DrawSphere(
                transform.TransformPoint(groundCheckOffset),
                groundCheckRadius
            );
        }
        #endregion
    }
}
