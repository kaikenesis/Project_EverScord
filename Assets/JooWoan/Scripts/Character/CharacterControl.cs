using System.Collections.Generic;
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
        public ISkillAction CurrentSkillInfo                            { get; private set; }
        public Vector3 MouseRayHitPos                                   { get; private set; }
        public Vector3 MoveVelocity                                     { get; private set; }
        public EJob CharacterJob                                        { get; private set; }

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

        public float CurrentHealth
        {
            get { return currentHealth; }
            set
            {
                currentHealth = value;
            }
        }

        void Awake()
        {
            photonView       = GetComponent<PhotonView>();

            PlayerTransform  = transform;
            PhysicsControl   = new CharacterPhysics(gravity, mass);

            controller       = GetComponent<CharacterController>();
            AnimationControl = GetComponent<CharacterAnimation>();

            uiHub            = GameObject.FindGameObjectWithTag(ConstStrings.TAG_UIROOT).transform;
            cameraHub        = GameObject.FindGameObjectWithTag(ConstStrings.TAG_CAMERAROOT).transform;
            
            PlayerUIControl  = Instantiate(uiPrefab, uiHub);
            CameraControl    = Instantiate(cameraPrefab, cameraHub);

            // Unity docs: Set skinwidth 10% of the Radius
            controller.skinWidth = controller.radius * SKIN_RATIO;
            
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
            SetSkills();
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
            MouseRayHitPos = Vector3.Lerp(MouseRayHitPos, remoteMouseRayHitPos, Time.deltaTime * 10f);
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
            movement = new Vector3(moveInput.x, 0, moveInput.z);

            Vector3 velocity = movement * speed;
            velocity.y = PhysicsControl.FallSpeed;

            if (PhysicsControl.IsImpactAdded)
                velocity = PhysicsControl.ImpactVelocity.normalized * speed;
            
            controller.Move(velocity * Time.deltaTime);
        }

        public void TrackAim()
        {
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
            weapon.SetGunPointDirection(gunPoint2MouseDir);

            Vector3 aimPosition = weapon.GunPoint.position + weapon.GunPoint.forward * weapon.WeaponRange;
            weapon.AimPoint.position = aimPosition;
        }

        private void RotateBody()
        {
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

                info.SkillAction.Activate();
                CurrentSkillInfo = skillList[i].SkillAction;

                if (PhotonNetwork.IsConnected)
                    photonView.RPC(nameof(SyncUseSkill), RpcTarget.Others, i);
                
                break;
            }
        }

        private void SetSkills()
        {
            if (!photonView.IsMine)
                return;

            for (int i = 0; i < skillList.Count; i++)
            {
                CharacterJob = GameManager.Instance.userData.job;
                skillList[i].Init(this, i, CharacterJob);

                if (PhotonNetwork.IsConnected)
                    photonView.RPC(nameof(SyncInitSkill), RpcTarget.Others, i, (int)CharacterJob);
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

        public void DecreaseHP(float amount)
        {
            CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
            
            if (PhotonNetwork.IsConnected)
                photonView.RPC(nameof(SyncHealth), RpcTarget.Others, currentHealth);
        }

        public void IncreaseHP(float amount)
        {
            CurrentHealth = Mathf.Min(maxHealth, CurrentHealth + amount);
            
            if (PhotonNetwork.IsConnected)
                photonView.RPC(nameof(SyncHealth), RpcTarget.Others, currentHealth);
        }

        public void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            GameManager.Instance.AddPlayerPhotonView(info.photonView);
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

        ////////////////////////////////////////  PUN RPC  //////////////////////////////////////////////////////

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
        private void SyncInitSkill(int index, int characterJob)
        {
            CharacterJob = (EJob)characterJob;
            skillList[index].Init(this, index, (EJob)characterJob);
        }

        [PunRPC]
        private void SyncUseSkill(int index)
        {
            CurrentSkillInfo = skillList[index].SkillAction;
            skillList[index].SkillAction.Activate();
        }

        [PunRPC]
        public void SyncCounterSkill(Vector3 mouseRayHitPos, bool toggle, int index)
        {
            CounterSkillAction skillAction = (CounterSkillAction)skillList[index].SkillAction;

            MouseRayHitPos = MouseRayHitPos;
            playerInputInfo.pressedLeftMouseButton = true;
        }

        [PunRPC]
        public void SyncGrenadeSkill(Vector3 mouseRayHitPos, Vector3 throwDir, int index)
        {
            GrenadeSkillAction skillAction = (GrenadeSkillAction)skillList[index].SkillAction;
            skillAction.SyncGrenadeSkill(throwDir);

            MouseRayHitPos = mouseRayHitPos;
            playerInputInfo.pressedLeftMouseButton = true;
        }

        [PunRPC]
        private void SyncHealth(float health)
        {
            CurrentHealth = health;
        }

        ////////////////////////////////////////  PUN RPC  //////////////////////////////////////////////////////

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
