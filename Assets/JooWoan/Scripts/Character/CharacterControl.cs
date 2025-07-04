using UnityEngine.AddressableAssets;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using Photon.Pun;
using EverScord.Weapons;
using EverScord.UI;
using EverScord.GameCamera;
using EverScord.Skill;
using EverScord.Effects;
using EverScord.Armor;
using EverScord.Augment;

namespace EverScord.Character
{
    public class CharacterControl : MonoBehaviour, IPunInstantiateMagicCallback, IPunObservable
    {
        private const float SKIN_RATIO = 0.1f;
        private const float LOWHEALTH_THRESHOLD = 0.1f;
        private const float REMOTE_LERP_VALUE = 10f;
        private const float BODY_CENTER = 2f;
        private const float BODY_ROTATESTART_ANGLE = 2f;

        [Header("Character")]
        [SerializeField] private float gravity;
        [SerializeField] private float mass;
        [SerializeField] private float bodyRotateSpeed;

        [Header("Ground Check")]
        [SerializeField] private float groundCheckRadius;
        [SerializeField] private Vector3 groundCheckOffset;

        [Header("Weapon")]
        [SerializeField] private Weapon weapon;

        [Header("Sound")]
        [SerializeField] private AssetReference frontFootstep;
        [SerializeField] private AssetReference backFootstep;

        [Header("Skill")]
        [SerializeField] private List<SkillActionInfo> skillList;

        [Header("UI")]
        [SerializeField] private PlayerUI uiPrefab;

        [Header("Camera")]
        [SerializeField] private CharacterCamera cameraPrefab;

        [Header("Rig")]
        [SerializeField] private CharacterRigControl rigLayerPrefab;

        public static CharacterControl CurrentClientCharacter           { get; private set; }
        public CharacterStat Stats                                      { get; private set; }
        public PlayerUI PlayerUIControl                                 { get; private set; }
        public CharacterRigControl RigControl                           { get; private set; }
        public CharacterAnimation AnimationControl                      { get; private set; }
        public CharacterPhysics PhysicsControl                          { get; private set; }
        public CharacterCamera CameraControl                            { get; private set; }
        public Transform PlayerTransform                                { get; private set; }
        public SkinnedMeshRenderer[] SkinRenderers                      { get; private set; }
        public SkillAction CurrentSkillAction                           { get; private set; }
        public BlinkEffect BlinkEffects                                 { get; private set; }
        public UIMarker UIMarker                                        { get; private set; }
        public PlayerData.ECharacter CharacterType                      { get; private set; }
        public PlayerData.EJob CharacterJob                             { get; private set; }
        public List<CharacterBuff> BuffList                             { get; private set; }
        public IDictionary<CharState, Debuff> DebuffDict                { get; private set; }
        public CharState State                                          { get; private set; }
        public IHelmet CharacterHelmet                                  { get; private set; }
        public IVest CharacterVest                                      { get; private set; }
        public IShoes CharacterShoes                                    { get; private set; }
        public LayerMask OriginalSkinLayer                              { get; private set; }
        public Vector3 MouseRayHitPos                                   { get; private set; }
        public Vector3 MoveVelocity                                     { get; private set; }
        public string Nickname                                          { get; private set; }
        public float DealtDamage                                        { get; private set; }
        public float DealtHeal                                          { get; private set; }
        public int KillCount                                            { get; private set; }

        public Weapon PlayerWeapon => weapon;
        public PhotonView CharacterPhotonView => photonView;
        public CharacterController Controller => controller;
        public InputInfo PlayerInputInfo => playerInputInfo;
        public List<SkillActionInfo> SkillList => skillList;
        public Vector3 LookDir => lookDir;

        public static Action OnPhotonViewListUpdated = delegate { };
        public static Action<int, bool, Vector3> OnCheckAlive = delegate { };

        private GameObject deathEffect, reviveEffect, beamEffect;
        private ParticleSystem hitEffect1, hitEffect2, healEffect;

        private Action onDecreaseHealth;
        private PhotonView photonView;
        private ReviveCircle reviveCircle;
        private CharacterController controller;
        private Coroutine deathCoroutine, hpRegenCoroutine, footstepCoroutine;
        private WaitForSeconds waitHpRegen, waitFootstep;
        private Vector3 movement, lookDir, moveInput, moveDir;
        private Vector3 remoteMouseRayHitPos;
        private LayerMask groundAndEnemyLayer;
        private InputInfo playerInputInfo;

        void Awake()
        {
            PlayerTransform  = transform;

            photonView       = GetComponent<PhotonView>();
            controller       = GetComponent<CharacterController>();
            AnimationControl = GetComponent<CharacterAnimation>();
            SkinRenderers    = GetComponentsInChildren<SkinnedMeshRenderer>();
            UIMarker         = gameObject.AddComponent<UIMarker>();
            Stats            = gameObject.AddComponent<CharacterStat>();

            BuffList         = new List<CharacterBuff>();
            DebuffDict       = new Dictionary<CharState, Debuff>();
            CharacterHelmet  = new Helmet();
            CharacterVest    = new Vest();
            CharacterShoes   = new Shoes();
            PhysicsControl   = new CharacterPhysics(gravity, mass);
            waitHpRegen      = new WaitForSeconds(1f);
            waitFootstep     = new WaitForSeconds(AnimationControl.AnimInfo.RunForward.length * 0.48f);
            playerInputInfo  = new InputInfo();

            // Unity docs: Set skinwidth 10% of the Radius
            controller.skinWidth = controller.radius * SKIN_RATIO;
            
            if (SkinRenderers.Length > 0)
                OriginalSkinLayer = 1 << SkinRenderers[0].gameObject.layer;

            if (photonView.IsMine)
            {
                PlayerUIControl = Instantiate(uiPrefab, PlayerUI.Root);
                PlayerUIControl.Init(weapon.IconPrefab);

                CameraControl = Instantiate(cameraPrefab, CharacterCamera.Root);
                CameraControl.Init(PlayerTransform, photonView.IsMine);

                CharacterType = GameManager.Instance.PlayerData.character;

                CurrentClientCharacter = this;
            }

            UIMarker.Initialize(PointMarkData.EType.Player);
            AnimationControl.Init(photonView);
            weapon.Init(this);

            RigControl = Instantiate(rigLayerPrefab, AnimationControl.Anim.transform);
            RigControl.Init(AnimationControl.Anim.transform, GetComponent<Animator>(), weapon);
            RigControl.SetAimWeight(false);

            BlinkEffects = BlinkEffect.Create(transform, GameManager.HurtBlinkInfo);
            groundAndEnemyLayer = GameManager.GroundLayer | GameManager.EnemyLayer;

            CharacterJob = GameManager.Instance.PlayerData.job;
        }

        void OnEnable()
        {
            EnableRegenerateHP(true);
        }

        void Start()
        {
            InitBasicInfo();
            SetReviveCircle();
            SetEffects();
            SetPortraits();
            EnableRegenerateHP(true);

            //OnCheckAlive?.Invoke(photonView.ViewID, IsDead, Vector3.zero);
        }

        void Update()
        {
            UIMarker.UpdatePosition(PlayerTransform.position);

            if (!photonView.IsMine)
            {
                LerpRemoteInfo();
                return;
            }

            SetInput();

            if (IsDead || IsInteractingUI)
                return;

            SetMovingDirection();

            PhysicsControl.ApplyGravity(this);
            PhysicsControl.ApplyImpact(this);

            Move();
            AnimationControl.AnimateMovement(this, moveDir);
            PlayFootstepSounds();

            TrackAim();
            RotateBody();

            weapon.TryReload(this);
            weapon.Shoot(this);

            UseSkills();
        }

        void OnDisable()
        {
            IsAiming = false;
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
            if (!CanMove)
                return;

            movement = new Vector3(moveInput.x, 0, moveInput.z);

            Vector3 velocity = movement * Stats.Speed;
            velocity.y = PhysicsControl.FallSpeed;

            if (PhysicsControl.IsImpactAdded)
                velocity = PhysicsControl.ImpactVelocity.normalized * Stats.Speed;
            
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

                    if (enemy is NController nctrl && nctrl.isDead)
                        continue;
                    
                    OutlineControl.EnableEnemyOutline(photonView, enemy);
                    enemyFlag = true;
                }
                else if ((GameManager.GroundLayer & hitLayerMask) != 0)
                {
                    hit = hits[i];
                    groundFlag = true;
                }
            }

            if (!enemyFlag)
                OutlineControl.DisableEnemyOutline(photonView);

            return groundFlag;
        }

        public void TrackAim()
        {
            if (!CanRotate)
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
            if (!CanRotate)
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

        private void PlayFootstepSounds()
        {
            if (footstepCoroutine == null && IsMoving)
                footstepCoroutine = StartCoroutine(FootstepSound());

            else if (footstepCoroutine != null && !IsMoving)
            {
                StopCoroutine(footstepCoroutine);
                footstepCoroutine = null;
            }
        }

        private IEnumerator FootstepSound()
        {
            while (IsMoving)
            {
                if (moveDir.z > 0)
                    SoundManager.Instance.PlaySound(frontFootstep.AssetGUID);
                else
                    SoundManager.Instance.PlaySound(backFootstep.AssetGUID);

                yield return waitFootstep;
            }
        }

        private void InitBasicInfo()
        {
            if (!photonView.IsMine)
                return;

            Stats.InitBaseStat(this);

            for (int i = 0; i < skillList.Count; i++)
            {
                skillList[i].Init(this, i, CharacterJob);

                if (PhotonNetwork.IsConnected)
                    photonView.RPC(nameof(SyncBasicInfo), RpcTarget.Others, i, (int)CharacterJob, (int)CharacterType);
            }
        }

        private void SetPortraits()
        {
            if (PhotonNetwork.IsMasterClient && GameManager.Instance.PlayerDict.Count >= PhotonNetwork.CurrentRoom.PlayerCount)
                photonView.RPC(nameof(CreatePortrait), RpcTarget.All);
        }

        private void SetReviveCircle()
        {
            if (!photonView.IsMine)
                return;

            if (PhotonNetwork.IsConnected)
                photonView.RPC(nameof(SyncInitReviveCircle), RpcTarget.All, photonView.ViewID);
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

        public void SetArmor(IArmor newArmor)
        {
            switch (newArmor)
            {
                case IHelmet newHelmet:
                    CharacterHelmet = newHelmet;
                    break;

                case IVest newVest:
                    CharacterVest = newVest;
                    break;

                case IShoes newShoes:
                    CharacterShoes = newShoes;
                    break;
            }
        }

        public void SetState(SetCharState mode, CharState state = CharState.NONE)
        {
            if (state == CharState.NONE && mode != SetCharState.CLEAR)
                return;
            
            switch (mode)
            {
                case SetCharState.ADD:
                    State |= state;
                    
                    DebuffDict[state] = Debuff.GetDebuff(this, state, RemoveDebuff);
                    break;

                case SetCharState.REMOVE:
                    State &= ~state;

                    if (DebuffDict.ContainsKey(state))
                    {
                        DebuffDict[state]?.RemoveDebuff();
                        DebuffDict[state] = null;
                        DebuffDict.Remove(state);
                    }
                    break;

                case SetCharState.CLEAR:
                    State = 0;

                    List<Debuff> debuffs = DebuffDict.Values.ToList();

                    for (int i = debuffs.Count - 1; i >= 0; i--)
                        debuffs[i]?.RemoveDebuff();
                    break;

                default:
                    break;
            }
        }

        public bool HasState(CharState state)
        {
            return (State & state) != 0;
        }

        public void ApplyBuff(BuffType type, float duration)
        {
            CharacterBuff buff = CharacterBuff.GetBuff(this, type, duration, RemoveBuff);

            if (buff != null)
                BuffList.Add(buff);
        }

        private void RemoveBuff()
        {
            for (int i = BuffList.Count - 1; i >= 0; i--)
            {
                if (BuffList[i].IsBuffOver)
                    BuffList.RemoveAt(i);
            }
        }

        public void ApplyDebuff(CharState state, int count)
        {
            if (IsInvincible)
                return;

            if (DebuffDict.ContainsKey(state) && DebuffDict[state] != null)
                return;
            
            if (!PhotonNetwork.IsConnected)
                return;
            
            photonView.RPC(nameof(SyncApplyDebuff), RpcTarget.All, (int)state, count);
        }

        private void RemoveDebuff(CharState state)
        {
            if (PhotonNetwork.IsConnected)
                photonView.RPC(nameof(SyncRemoveDebuff), RpcTarget.All, (int)state);
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

        public void DecreaseHP(float amount, bool isMaxHPDamage = false)
        {
            bool isInvincible = HasState(CharState.INVINCIBLE);

            if (isInvincible)
                BlinkEffects.ChangeBlinkTemporarily(GameManager.InvincibleBlinkInfo);
            
            BlinkEffects.Blink();
            PlayHitEffects();

            if (PhotonNetwork.IsConnected)
                photonView.RPC(nameof(SyncHitEffects), RpcTarget.Others, false, isInvincible);

            if (IsDead)
                return;

            if (!isInvincible && isMaxHPDamage == false)
                Stats.CurrentHealth = Mathf.Max(0, Stats.CurrentHealth - amount);

            if (!isInvincible && isMaxHPDamage == true)
                Stats.CurrentHealth = Mathf.Max(0, Stats.CurrentHealth - (Stats.MaxHealth * 0.01f * amount));

            onDecreaseHealth?.Invoke();

            if (PhotonNetwork.IsConnected)
                photonView.RPC(nameof(SyncHealth), RpcTarget.Others, Stats.CurrentHealth, false);
        }

        public void IncreaseHP(CharacterControl activator, float amount, bool isExternalHeal = false, bool playEffect = true)
        {
            if (IsDead)
                return;

            if (playEffect)
                PlayHealEffects();

            Stats.CurrentHealth = Mathf.Min(Stats.MaxHealth, Stats.CurrentHealth + amount);

            if (activator.CharacterPhotonView.IsMine && isExternalHeal)
                activator.IncreaseDealtHeal(amount);

            if (PhotonNetwork.IsConnected && (photonView.IsMine || isExternalHeal))
            {
                if (playEffect)
                    photonView.RPC(nameof(SyncHitEffects), RpcTarget.Others, true, false);

                photonView.RPC(nameof(SyncHealth), RpcTarget.All, Stats.CurrentHealth, true);
            }
        }

        public void EnableRegenerateHP(bool state)
        {
            if (!Stats.IsInitialized)
                return;

            if (!CharacterPhotonView.IsMine)
                return;

            if (hpRegenCoroutine != null)
                StopCoroutine(hpRegenCoroutine);
            
            if (state)
                hpRegenCoroutine = StartCoroutine(RegenerateHP());
        }

        private IEnumerator RegenerateHP()
        {
            while (!IsDead)
            {
                IncreaseHP(this, Stats.HealthRegen, isExternalHeal: false, playEffect: false);
                yield return waitHpRegen;
            }
        }

        public void IncreaseDealtDamage(float amount)
        {
            DealtDamage += amount;
        }

        public void IncreaseDealtHeal(float amount)
        {
            DealtHeal += amount;
        }

        public void IncreaseKillCount()
        {
            ++KillCount;
        }

        public void StartDeath()
        {
            deathCoroutine = StartCoroutine(HandleDeath());
        }

        public IEnumerator HandleDeath()
        {
            SetState(SetCharState.CLEAR);
            SetState(SetCharState.ADD, CharState.DEATH);

            controller.enabled = false;

            if (hpRegenCoroutine != null)
                StopCoroutine(hpRegenCoroutine);

            Instantiate(deathEffect, transform.position, Quaternion.identity);
            BlinkEffects.LoopBlink(false);
            
            RigControl.SetAimWeight(false);
            RigControl.SetMainRigWeight(false);

            AnimationControl.SetUpperMask(false, true);
            AnimationControl.Play(AnimationControl.AnimInfo.Death);

            OutlineControl.SetCharacterOutline(this, true);

            OnCheckAlive?.Invoke(photonView.ViewID, IsDead, transform.position);
            UIMarker.ToggleDeathIcon();

            SoundManager.Instance.PlaySound(ConstStrings.SFX_DEATH_1);
            SoundManager.Instance.PlaySound(ConstStrings.SFX_DEATH_2);

            if (photonView.IsMine)
            {
                GameManager.Instance.GameOverController.CheckGameOver();
            }

            yield return new WaitForSeconds(AnimationControl.AnimInfo.Death.length);

            EnableReviveCircle(true);

            if (PhotonNetwork.IsConnected && photonView.IsMine)
                photonView.RPC(nameof(SyncReviveCircle), RpcTarget.Others, true);
        }

        public IEnumerator HandleRevival()
        {
            if (deathCoroutine != null)
                StopCoroutine(deathCoroutine);

            yield return new WaitForEndOfFrame();

            SoundManager.Instance.PlaySound(ConstStrings.SFX_PLAYER_REVIVE);
            SoundManager.Instance.PlaySound(ConstStrings.SFX_UNI_SKILL);

            SetState(SetCharState.CLEAR);
            SetState(SetCharState.ADD, CharState.INVINCIBLE);

            Instantiate(reviveEffect, PlayerTransform.position, Quaternion.identity);
            Instantiate(beamEffect, PlayerTransform.position, Quaternion.identity);

            PlayHealEffects();
            Stats.CurrentHealth = Stats.MaxHealth;

            AnimationControl.Play(AnimationControl.AnimInfo.Revive);
            EnableReviveCircle(false);

            OutlineControl.SetCharacterOutline(this, false);
            
            yield return new WaitForSeconds(AnimationControl.AnimInfo.Revive.length);

            RigControl.SetMainRigWeight(true);
            AnimationControl.CrossFade(new AnimationParam(AnimationControl.AnimInfo.Idle.name, 0.25f));

            for (float t = 0; t < 0.5f; t += Time.deltaTime)
                yield return null;

            controller.enabled = true;

            SetState(SetCharState.CLEAR);

            OnCheckAlive?.Invoke(photonView.ViewID, IsDead, transform.position);
            UIMarker.ToggleDeathIcon();

            EnableRegenerateHP(true);
        }

        public void PlayHitEffects()
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

            if (IsInvincible)
                SoundManager.Instance.PlaySound(ConstStrings.SFX_INVINCIBLE_HIT, 1, true);
            else
                SoundManager.Instance.PlaySound(ConstStrings.SFX_PLAYER_HIT, 1, true);

            if (CharacterType == PlayerData.ECharacter.Us)
                SoundManager.Instance.PlaySound(ConstStrings.SFX_US_HIT, 1, true);
        }

        private void PlayHealEffects()
        {
            if (healEffect == null)
                return;

            healEffect.transform.position = PlayerTransform.position;

            if (!healEffect.gameObject.activeSelf)
                healEffect.gameObject.SetActive(true);
            
            healEffect.Emit(1);
            SoundManager.Instance.PlaySound(ConstStrings.SFX_HEAL_1, 1, true);
            SoundManager.Instance.PlaySound(ConstStrings.SFX_HEAL_2, 1, true);
        }

        public void Teleport(Vector3 position)
        {
            PlayerTransform.position = position;
        }

        public void SetActive(bool state)
        {
            gameObject.SetActive(state);
        }

        public void EnableReviveCircle(bool state)
        {
            reviveCircle.gameObject.SetActive(state);
        }

        private void CancelAction()
        {
            weapon.SetShootingStance(this, false, true);

            for (int i = 0; i < skillList.Count; i++)
                skillList[i].SkillAction.ExitSkill();
        }

        public bool CanMove
        {
            get
            {
                if (HasState(CharState.SKILL_STANCE))
                    return false;

                if (HasState(CharState.TELEPORTING))
                    return false;

                if (HasState(CharState.STUNNED))
                    return false;

                return true;
            }
        }

        public bool CanRotate
        {
            get
            {
                if (HasState(CharState.RIGID_ANIMATING))
                    return false;

                if (HasState(CharState.TELEPORTING))
                    return false;

                if (HasState(CharState.STUNNED))
                    return false;

                return true;
            }
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
            get { return Stats.CurrentHealth <= Stats.MaxHealth * LOWHEALTH_THRESHOLD; }
        }

        public bool IsDead
        {
            get { return HasState(CharState.DEATH); }
        }

        public bool IsStunned
        {
            get { return HasState(CharState.STUNNED); }
        }

        public bool IsInteractingUI
        {
            get { return HasState(CharState.INTERACTING_UI); }
        }

        public bool IsInvincible
        {
            get { return HasState(CharState.INVINCIBLE); }
        }

        public bool IsAiming { get; private set; }
        public bool IsShooting => playerInputInfo.holdLeftMouseButton;
        public bool IsMoving => (CanMove && moveInput.magnitude > 0) || PhysicsControl.IsImpactAdded;

        #region Photon
        ////////////////////////////////////////  PUN RPC  //////////////////////////////////////////////////////

        public void OnPhotonInstantiate(PhotonMessageInfo info)
        {
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
        private void SyncBasicInfo(int index, int characterJob, int characterType)
        {
            CharacterJob = (PlayerData.EJob)characterJob;
            CharacterType = (PlayerData.ECharacter)characterType;

            Stats.InitBaseStat(this);
            skillList[index].Init(this, index, (PlayerData.EJob)characterJob);

            EnableRegenerateHP(true);
        }

        [PunRPC]
        private void SyncUseSkill(int index)
        {
            CurrentSkillAction = skillList[index].SkillAction;
            skillList[index].SkillAction.Activate();
        }

        [PunRPC]
        private void SyncApplyDebuff(int state, int count)
        {
            CancelAction();
            SetState(SetCharState.ADD, (CharState)state);

            StunnedDebuff debuff = DebuffDict[CharState.STUNNED] as StunnedDebuff;

            if (debuff != null)
                debuff.SetCount(count);
        }

        [PunRPC]
        private void SyncRemoveDebuff(int state)
        {
            CharState debuffState = (CharState)state;
            SetState(SetCharState.REMOVE, debuffState);
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
            Stats.CurrentHealth = health;

            if (!isIncreasing)
                onDecreaseHealth?.Invoke();
        }

        [PunRPC]
        public void SyncState(int state)
        {
            State = (CharState)state;
        }

        [PunRPC]
        private void SyncHitEffects(bool isIncreasing, bool isInvincible)
        {
            if (isIncreasing)
                PlayHealEffects();
            else
            {
                if (isInvincible)
                    BlinkEffects.ChangeBlinkTemporarily(GameManager.InvincibleBlinkInfo);

                BlinkEffects.Blink();
                PlayHitEffects();
            }
        }

        [PunRPC]
        private void SyncInitReviveCircle(int viewID)
        {
            if (photonView.ViewID != viewID)
                return;

            var circlePrefab = ResourceManager.Instance.GetAsset<GameObject>(AssetReferenceManager.ReviveCircle_ID);
            reviveCircle = Instantiate(circlePrefab, PlayerTransform).GetComponent<ReviveCircle>();

            reviveCircle.Init(photonView.ViewID);
            reviveCircle.transform.SetParent(PlayerUI.Root.parent);
            reviveCircle.SetLocalHeight(0.02f);
            reviveCircle.gameObject.SetActive(false);
        }

        [PunRPC]
        private void SyncReviveCircle(bool state)
        {
            EnableReviveCircle(state);
        }

        [PunRPC]
        public void SyncExitCircle(int viewID)
        {
            if (photonView.ViewID != viewID)
                return;

            reviveCircle.SyncExitCircle();
        }

        [PunRPC]
        public void SyncInteractStunDebuff()
        {
            if (!DebuffDict.ContainsKey(CharState.STUNNED))
                return;

            if (DebuffDict[CharState.STUNNED] is not StunnedDebuff debuff)
                return;

            debuff.DecreaseCount();
        }

        [PunRPC]
        public void SyncPlayerResult(int killCount, float dealtDamage, float dealtHeal, string nickname)
        {
            KillCount = killCount;
            DealtDamage = dealtDamage;
            DealtHeal = dealtHeal;
            Nickname = nickname;

            GameManager.Instance.ResultControl.IncreaseReadyCount();
        }

        [PunRPC]
        private void CreatePortrait()
        {
            OnPhotonViewListUpdated?.Invoke();
        }

        [PunRPC]
        public void SyncArmor(string helmetTag, string vestTag, string shoesTag, int enhanceIndex)
        {
            GameManager.Instance.AugmentControl.SyncPlayerArmor(this, helmetTag, vestTag, shoesTag, enhanceIndex);
        }

        [PunRPC]
        public void SyncAlterationBonus(int bonusType, float additive, float multiplicative)
        {
            Stats.SetAlterationBonus((StatType)bonusType, additive, multiplicative);
        }

        ////////////////////////////////////////  PUN RPC  //////////////////////////////////////////////////////
        #endregion
    }
}
