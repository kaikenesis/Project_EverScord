using System.Collections.Generic;
using System;
using UnityEngine;
using Photon.Pun;
using EverScord.Weapons;
using EverScord.UI;
using EverScord.GameCamera;
using EverScord.Skill;

namespace EverScord.Character
{
    public class CharacterControl : MonoBehaviour, IPunInstantiateMagicCallback, IPunObservable
    {
        private const float SKIN_RATIO = 0.1f;
        private const float REMOTE_LERP_VALUE = 10f;

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
        public SkillAction CurrentSkillAction                           { get; private set; }
        public Vector3 MouseRayHitPos                                   { get; private set; }
        public Vector3 MoveVelocity                                     { get; private set; }
        public PlayerData.EJob CharacterJob                         { get; private set; }
        public CharState State                                     { get; private set; }

        private InputInfo playerInputInfo = new InputInfo();
        public InputInfo PlayerInputInfo => playerInputInfo;
        public Weapon PlayerWeapon => weapon;
        public PhotonView CharacterPhotonView => photonView;
        public CharacterController Controller => controller;
        public Vector3 LookDir => lookDir;
        public float CharacterSpeed => speed;

        private PhotonView photonView;
        private CharacterController controller;
        private Transform uiHub, cameraHub;
        private Vector3 movement, lookDir, moveInput, moveDir;
        private Vector3 remoteMouseRayHitPos;
        private Action onDecreaseHealth;

        private SkinnedMeshRenderer[] skinRenderers;
        private int originalSkinLayer;

        public float CurrentHealth
        {
            get { return currentHealth; }
            set
            {
                currentHealth = value;
                // Change Helath UI
            }
        }

        void Awake()
        {
            photonView       = GetComponent<PhotonView>();

            PlayerTransform  = transform;
            PhysicsControl   = new CharacterPhysics(gravity, mass);

            controller       = GetComponent<CharacterController>();
            AnimationControl = GetComponent<CharacterAnimation>();
            skinRenderers    = GetComponentsInChildren<SkinnedMeshRenderer>();

            uiHub            = GameObject.FindGameObjectWithTag(ConstStrings.TAG_UIROOT).transform;
            cameraHub        = GameObject.FindGameObjectWithTag(ConstStrings.TAG_CAMERAROOT).transform;
            
            PlayerUIControl  = Instantiate(uiPrefab, uiHub);
            CameraControl    = Instantiate(cameraPrefab, cameraHub);

            // Unity docs: Set skinwidth 10% of the Radius
            controller.skinWidth = controller.radius * SKIN_RATIO;
            
            if (skinRenderers.Length > 0)
                originalSkinLayer = skinRenderers[0].gameObject.layer;

            CurrentHealth = maxHealth;

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

            CameraControl.Init(PlayerTransform);
        }

        void Start()
        {
            GameManager.Instance.InitControl(this);
            SetJobAndSkills();
        }

        void Update()
        {
            if (!photonView.IsMine)
            {
                LerpRemoteInfo();
                return;
            }

            SetInput();
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

        public void TrackAim()
        {
            if (HasState(CharState.RIGID_ANIMATING))
                return;
            
            Ray ray = CameraControl.Cam.ScreenPointToRay(playerInputInfo.mousePosition);

            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, GameManager.GroundLayer))
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

            if (angle > 2f)
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
                CharacterJob = GameManager.Instance.PlayerData.job;
                skillList[i].Init(this, i, CharacterJob);

                if (PhotonNetwork.IsConnected)
                    photonView.RPC(nameof(SyncJobAndSkills), RpcTarget.Others, i, (int)CharacterJob);
            }
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
            onDecreaseHealth?.Invoke();
            
            if (HasState(CharState.INVINCIBLE))
                return;
            
            CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
            
            if (PhotonNetwork.IsConnected)
                photonView.RPC(nameof(SyncHealth), RpcTarget.Others, currentHealth, false);
        }

        public void IncreaseHP(float amount)
        {
            CurrentHealth = Mathf.Min(maxHealth, CurrentHealth + amount);

            GameObject healEffect = ResourceManager.Instance.GetAsset<GameObject>(ConstStrings.KEY_HEAL_EFFECT);
            var effect = Instantiate(healEffect, transform);
            effect.transform.position = transform.position;

            if (PhotonNetwork.IsConnected)
                photonView.RPC(nameof(SyncHealth), RpcTarget.Others, currentHealth, true);
        }

        public void SetCharacterOutline(bool state)
        {
            int OutlineLayerNumber = Mathf.RoundToInt(Mathf.Log(GameManager.OutlineLayer.value, 2));

            for (int i = 0; i < skinRenderers.Length; i++)
            {
                if (state)
                    skinRenderers[i].gameObject.layer = OutlineLayerNumber;
                else
                    skinRenderers[i].gameObject.layer = originalSkinLayer;
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

        public bool IsAiming { get; private set; }
        public bool IsShooting => playerInputInfo.holdLeftMouseButton;
        public bool IsMoving => moveInput.magnitude > 0 || PhysicsControl.IsImpactAdded;

        #region Photon
        ////////////////////////////////////////  PUN RPC  //////////////////////////////////////////////////////

        public void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            GameManager.Instance.AddPlayerPhotonView(info.photonView);
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

            skillAction.Predictor.SyncInfo(thrownPosition, groundDirection, initialVelocity, trajectoryAngle, estimatedTime);
            skillAction.StartCoroutine(skillAction.ThrowObject());
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

            if (isIncreasing)
            {
                GameObject healEffect = ResourceManager.Instance.GetAsset<GameObject>(ConstStrings.KEY_HEAL_EFFECT);
                var effect = Instantiate(healEffect, transform);
                effect.transform.position = transform.position;
            }
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
