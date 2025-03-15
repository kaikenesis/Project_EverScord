using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
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
        private const float REMOTE_LERP_VALUE = 10f;
        private const float BODY_CENTER = 2f;
        private const float BODY_ROTATESTART_ANGLE = 2f;
        private const float SPEED_FACTOR = 0.01f * 2f;

        [Header("Character")]
        [SerializeField] private float gravity;
        [SerializeField] private float mass;
        [SerializeField] private float bodyRotateSpeed;
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

        public static CharacterControl CurrentClientCharacter           { get; private set; }
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
        public float MaxHealth                                          { get; private set; }
        [field: SerializeField] public float Speed                      { get; private set; }
        [field: SerializeField] public float Attack                     { get; private set; }
        [field: SerializeField] public float CriticalHitChance          { get; private set; }
        [field: SerializeField] public float Defense                    { get; private set; }
        [field: SerializeField] public float Heal                       { get; private set; }
        [field: SerializeField] public float HealthRegen                { get; private set; }
        [field: SerializeField] public float CooldownDecrease           { get; private set; }
        [field: SerializeField] public float ReloadSpeedDecrease        { get; private set; }
        [field: SerializeField] public float SkillDamageIncrease        { get; private set; }
        public float HealIncrease                                       { get; private set; }
        public float DealtDamage                                        { get; private set; }
        public float DealtHeal                                          { get; private set; }
        public int KillCount                                            { get; private set; }

        public Weapon PlayerWeapon => weapon;
        public PhotonView CharacterPhotonView => photonView;
        public CharacterController Controller => controller;
        public InputInfo PlayerInputInfo => playerInputInfo;
        public Vector3 LookDir => lookDir;

        public static Action OnPhotonViewListUpdated = delegate { };
        public static Action<int, bool, Vector3> OnCheckAlive = delegate { };
        public static Action<float> OnHealthUpdated = delegate { };

        private static GameObject deathEffect, reviveEffect, beamEffect;
        private static ParticleSystem hitEffect1, hitEffect2;

        private Action onDecreaseHealth;
        private PhotonView photonView;
        private ParticleSystem healEffect;
        private ReviveCircle reviveCircle;
        private CharacterController controller;
        private Coroutine deathCoroutine, hpRegenCoroutine;
        private WaitForSeconds waitHpRegen;
        private Vector3 movement, lookDir, moveInput, moveDir;
        private Vector3 remoteMouseRayHitPos;
        private LayerMask groundAndEnemyLayer;
        private InputInfo playerInputInfo;

        public float CurrentHealth
        {
            get { return currentHealth; }
            set
            {
                bool previousIsLowHealth = IsLowHealth;

                currentHealth = value;

                bool afterIsLowHealth = IsLowHealth;

                if (!IsDead && currentHealth <= 0)
                {
                    deathCoroutine = StartCoroutine(HandleDeath());
                    BlinkEffects.StopAllBlinks();
                }
                else if (previousIsLowHealth != afterIsLowHealth)
                    BlinkEffects.LoopBlink(IsLowHealth);

                if (photonView.IsMine)
                {
                    PlayerUIControl.SetGrayscaleScreen(currentHealth);
                    PlayerUIControl.SetBloodyScreen(currentHealth, afterIsLowHealth);
                }
            }
        }

        void Awake()
        {
            PlayerTransform  = transform;

            PhysicsControl   = new CharacterPhysics(gravity, mass);
            BuffList         = new List<CharacterBuff>();
            DebuffDict       = new Dictionary<CharState, Debuff>();
            CharacterHelmet  = new Helmet(10, 10, 10, 10, 10);
            CharacterVest    = new Vest(10, 10, 10);
            CharacterShoes   = new Shoes(10, 10, 10);

            waitHpRegen      = new WaitForSeconds(1f);
            playerInputInfo  = new InputInfo();

            photonView       = GetComponent<PhotonView>();
            controller       = GetComponent<CharacterController>();
            AnimationControl = GetComponent<CharacterAnimation>();
            SkinRenderers    = GetComponentsInChildren<SkinnedMeshRenderer>();
            UIMarker         = gameObject.AddComponent<UIMarker>();

            // Unity docs: Set skinwidth 10% of the Radius
            controller.skinWidth = controller.radius * SKIN_RATIO;
            
            if (SkinRenderers.Length > 0)
                OriginalSkinLayer = 1 << SkinRenderers[0].gameObject.layer;

            UIMarker.Initialize(PointMarkData.EType.Player);
            AnimationControl.Init(photonView);
            weapon.Init(this);

            RigControl = Instantiate(rigLayerPrefab, AnimationControl.Anim.transform);
            RigControl.Init(AnimationControl.Anim.transform, GetComponent<Animator>(), weapon);
            RigControl.SetAimWeight(false);

            if (photonView.IsMine)
            {
                PlayerUIControl = Instantiate(uiPrefab, PlayerUI.Root);
                PlayerUIControl.transform.localPosition = new Vector3(170f, -80f, 0f);
                PlayerUIControl.transform.localScale = new Vector3(0.8f, 0.8f, 1f);
                PlayerUIControl.Init();

                CameraControl = Instantiate(cameraPrefab, CharacterCamera.Root);
                CameraControl.Init(PlayerTransform, photonView.IsMine);

                CharacterType = GameManager.Instance.PlayerData.character;

                CurrentClientCharacter = this;
            }

            AnimationControl.Init(photonView);
            weapon.Init(this);

            RigControl = Instantiate(rigLayerPrefab, AnimationControl.Anim.transform);
            RigControl.Init(AnimationControl.Anim.transform, GetComponent<Animator>(), weapon);
            RigControl.SetAimWeight(false);

            BlinkEffects = BlinkEffect.Create(transform, GameManager.HurtBlinkInfo);
            groundAndEnemyLayer = GameManager.GroundLayer | GameManager.EnemyLayer;

            CharacterJob = GameManager.Instance.PlayerData.job;
        }

        void Start()
        {
            InitBasicInfo();
            SetReviveCircle();
            SetEffects();
            SetPortraits();

            OnCheckAlive?.Invoke(photonView.ViewID, IsDead, Vector3.zero);
            OnHealthUpdated?.Invoke(CurrentHealth / Mathf.Max(0.01f, MaxHealth));

            if (photonView.IsMine)
                hpRegenCoroutine = StartCoroutine(RegenerateHP());
        }

        void Update()
        {
            if (photonView.IsMine && Input.GetKeyDown(KeyCode.F1))
                IncreaseHP(this, 10);

            if (Input.GetKeyDown(KeyCode.F3))
                DecreaseHP(1000);

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

            Vector3 velocity = movement * Speed;
            velocity.y = PhysicsControl.FallSpeed;

            if (PhysicsControl.IsImpactAdded)
                velocity = PhysicsControl.ImpactVelocity.normalized * Speed;
            
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

        private void InitBasicInfo()
        {
            if (!photonView.IsMine)
                return;

            for (int i = 0; i < skillList.Count; i++)
            {
                InitBaseStat();
                skillList[i].Init(this, i, CharacterJob);

                if (PhotonNetwork.IsConnected)
                    photonView.RPC(nameof(SyncBasicInfo), RpcTarget.Others, i, (int)CharacterJob, (int)CharacterType);
            }
        }

        private void InitBaseStat()
        {
            string tag = PlayerData.GetCharacterName(CharacterType);
            StatInfo info = StatData.StatInfoDict[tag];

            currentHealth = MaxHealth = info.health;
            HealthRegen = info.healthRegen;

            Speed = info.speed * SPEED_FACTOR;
            Attack = info.attack;
            Defense = info.defense;
            CriticalHitChance = info.critChance;

            if (CharacterJob == PlayerData.EJob.Healer)
            {
                Attack = info.supportAttack;
                Heal = info.attack;
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

        public void SetSpeed(float speed)
        {
            Speed = speed;
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

        public void SetStat(StatType type, float amount)
        {
            switch (type)
            {
                case StatType.MAXHEALTH:
                    MaxHealth = amount;
                    break;

                case StatType.SPEED:
                    Speed = amount;
                    break;

                case StatType.ATTACK:
                    Attack = amount;
                    break;

                case StatType.DEFENSE:
                    Defense = amount;
                    break;

                case StatType.HEAL:
                    Heal = amount;
                    break;

                case StatType.HEALTH_REGEN:
                    HealthRegen = amount;
                    break;

                case StatType.COOLDOWN_DECREASE:
                    CooldownDecrease = amount;
                    break;

                case StatType.RELOADSPEED_DECREASE:
                    ReloadSpeedDecrease = amount;
                    break;

                case StatType.SKILLDAMAGE_INCREASE:
                    SkillDamageIncrease = amount;
                    break;

                default:
                    break;
            }
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
            if (HasState(CharState.INVINCIBLE))
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

        public void DecreaseHP(float amount)
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

            if (!isInvincible)
                CurrentHealth = Mathf.Max(0, CurrentHealth - amount);

            onDecreaseHealth?.Invoke();

            if (photonView.IsMine)
                OnHealthUpdated?.Invoke(CurrentHealth / Math.Max(0.01f, MaxHealth));

            if (PhotonNetwork.IsConnected)
                photonView.RPC(nameof(SyncHealth), RpcTarget.Others, currentHealth, false);
        }

        public void IncreaseHP(CharacterControl activator, float amount, bool isExternalHeal = false, bool playEffect = true)
        {
            if (IsDead)
                return;

            if (playEffect)
                PlayHealEffects();

            CurrentHealth = Mathf.Min(MaxHealth, CurrentHealth + amount);

            if (activator.CharacterPhotonView.IsMine)
                activator.IncreaseDealtHeal(amount);

            if (PhotonNetwork.IsConnected && (photonView.IsMine || isExternalHeal))
            {
                if (playEffect)
                    photonView.RPC(nameof(SyncHitEffects), RpcTarget.Others, true, false);

                photonView.RPC(nameof(SyncHealth), RpcTarget.All, currentHealth, true);
            }
        }

        private IEnumerator RegenerateHP()
        {
            while (!IsDead)
            {
                IncreaseHP(this, HealthRegen, playEffect: false);
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

        private IEnumerator HandleDeath()
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

            yield return new WaitForSeconds(AnimationControl.AnimInfo.Death.length);

            EnableReviveCircle(true);

            if (photonView.IsMine)
                GameManager.Instance.GameOverController.CheckGameOver();

            if (PhotonNetwork.IsConnected && photonView.IsMine)
                photonView.RPC(nameof(SyncReviveCircle), RpcTarget.Others, true);
        }

        public IEnumerator HandleRevival()
        {
            if (deathCoroutine != null)
                StopCoroutine(deathCoroutine);

            yield return new WaitForEndOfFrame();

            SetState(SetCharState.CLEAR);
            SetState(SetCharState.ADD, CharState.INVINCIBLE);

            Instantiate(reviveEffect, PlayerTransform.position, Quaternion.identity);
            Instantiate(beamEffect, PlayerTransform.position, Quaternion.identity);

            PlayHealEffects();
            CurrentHealth = MaxHealth;

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

            hpRegenCoroutine = StartCoroutine(RegenerateHP());

            if (photonView.IsMine)
                OnHealthUpdated?.Invoke(CurrentHealth / Mathf.Max(0.01f, MaxHealth));
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
        }

        private void PlayHealEffects()
        {
            if (healEffect == null)
                return;

            healEffect.transform.position = PlayerTransform.position;

            if (!healEffect.gameObject.activeSelf)
                healEffect.gameObject.SetActive(true);
            
            healEffect.Emit(1);
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
            get { return currentHealth <= MaxHealth * 0.1f; }
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
            get { return HasState(CharState.SELECTING_AUGMENT); }
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

            InitBaseStat();
            skillList[index].Init(this, index, (PlayerData.EJob)characterJob);
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
            CurrentHealth = health;

            if (photonView.IsMine)
                OnHealthUpdated?.Invoke(CurrentHealth / Mathf.Max(0.01f, MaxHealth));

            if (!isIncreasing)
                onDecreaseHealth?.Invoke();
        }

        [PunRPC]
        public void SyncState(int state)
        {
            State = (CharState)state;
        }

        [PunRPC]
        public void SyncOnAugmentSelect()
        {
            AugmentPresenter.IncreaseSelectedPeople();
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

        ////////////////////////////////////////  PUN RPC  //////////////////////////////////////////////////////
        #endregion
    }
}
