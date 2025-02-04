using UnityEngine;
using EverScord.Weapons;
using Photon.Pun;
using EverScord.UI;
using EverScord.GameCamera;
using EverScord.Skill;

namespace EverScord.Character
{
    public class CharacterControl : MonoBehaviour, IPunInstantiateMagicCallback
    {
        [Header("Character")]
        [SerializeField] private float speed;
        [SerializeField] private float gravity;
        [SerializeField] private float mass;

        [Header("Ground Check")]
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private float groundCheckRadius;
        [SerializeField] private Vector3 groundCheckOffset;

        [Header("Animation")]
        [SerializeField] private Animator anim;
        [SerializeField] private AnimationInfo info;
        [SerializeField] private CharacterRigControl rigLayerPrefab;
        [SerializeField] private float transitionDampTime;
        [SerializeField] private float bodyRotateSpeed;
        [field: SerializeField] public float ShootStanceDuration        { get; private set; }
        public CharacterRigControl RigControl                           { get; private set; }

        [Header("Weapon")]
        [SerializeField] private GameObject playerWeapon;
        private Weapon weapon;
        public Weapon PlayerWeapon => weapon;

        [Header("Skill")]
        [SerializeField] private SkillActionInfo firstSkillActionInfo;
        [SerializeField] private SkillActionInfo secondSkillActionInfo;

        [Header("UI")]
        [SerializeField] private PlayerUI uiPrefab;
        public PlayerUI PlayerUIControl                                 { get; private set; }
        private Transform uiHub;

        public CharacterAnimation AnimationControl                      { get; private set; }
        public CharacterPhysics PhysicsControl                          { get; private set; }
        public CharacterCamera CameraControl                            { get; private set; }
        public Transform PlayerTransform                                { get; private set; }
        public InputInfo PlayerInputInfo                                { get; private set; }
        public Vector3 AimPosition                                      { get; private set; }
        public EJob CharacterEJob                                       { get; private set; }

        private CharacterController controller;
        private PhotonView photonView;
        private Vector3 movement, lookPosition, lookDir, moveInput, moveDir;

        void Awake()
        {
            photonView       = GetComponent<PhotonView>();
            controller       = GetComponent<CharacterController>();
            weapon           = playerWeapon.GetComponent<Weapon>();

            uiHub            = GameObject.FindGameObjectWithTag(ConstStrings.TAG_UIHUB).transform;

            CameraControl    = GameObject.FindGameObjectWithTag(ConstStrings.TAG_CHARACTERCAM)
                               .GetComponent<CharacterCamera>();

            PlayerUIControl  = Instantiate(uiPrefab, uiHub);
            RigControl       = Instantiate(rigLayerPrefab, anim.transform);

            PlayerTransform  = transform;

            AnimationControl = new CharacterAnimation(anim, info, transitionDampTime);
            PhysicsControl   = new CharacterPhysics(gravity, mass);

            // Unity docs: Set skinwidth 10% of the Radius
            controller.skinWidth = controller.radius * 0.1f;

            weapon.Init(PlayerUIControl.SetAmmoText);
            RigControl.Init(anim.transform, GetComponent<Animator>(), weapon);

            RigControl.SetAimWeight(false);
            CameraControl.Init(PlayerTransform, Camera.main);
            PlayerUIControl.Init(this);

            firstSkillActionInfo.Skill?.Init(this, ref firstSkillActionInfo);
            secondSkillActionInfo.Skill?.Init(this, ref firstSkillActionInfo);
        }

        void OnApplicationFocus(bool focus)
        {
            // Cursor.visible = !focus;
        }

        void Update()
        {
            SetInput();
            SetMovingDirection();

            PhysicsControl.ApplyGravity(this);
            Move();

            AnimationControl.AnimateMovement(this, moveDir);

            TrackAim();
            RotateBody();

            weapon.CooldownTimer();
            weapon.TryReload(this);
            weapon.Shoot(this);
            weapon.UpdateBullets(this, Time.deltaTime);

            UseSkills();
        }

        private void SetInput()
        {
            PlayerInputInfo = InputControl.ReceiveInput();
            PlayerInputInfo = InputControl.GetCameraRelativeInput(PlayerInputInfo, CameraControl.Cam);

            moveInput = PlayerInputInfo.cameraRelativeInput;
        }

        private void SetMovingDirection()
        {
            if (moveInput.magnitude > 1f)
                moveInput.Normalize();

            moveDir = PlayerTransform.InverseTransformDirection(moveInput);
        }

        private void Move()
        {
            if (photonView.IsMine == false)
                return;

            movement = new Vector3(moveInput.x, 0, moveInput.z);

            Vector3 velocity = movement * speed;
            velocity.y = PhysicsControl.FallSpeed;

            controller.Move(velocity * Time.deltaTime);
        }

        private void TrackAim()
        {
            Ray ray = CameraControl.Cam.ScreenPointToRay(PlayerInputInfo.mousePosition);

            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
                return;

            AimPosition = hit.point;
            lookPosition = hit.point;
            lookPosition.y = PlayerTransform.position.y;

            lookDir = lookPosition - PlayerTransform.position;
            float distance = lookDir.magnitude;

            lookDir.Normalize();

            if (distance < 0.5f)
                return;

            if (distance < weapon.MinAimDistance)
                lookPosition = PlayerTransform.position + lookDir * weapon.MinAimDistance;

            lookPosition.y = weapon.GunPoint.position.y;

            weapon.AimPoint.position = Vector3.Lerp(
                weapon.AimPoint.position,
                lookPosition,
                Time.deltaTime * weapon.AimSensitivity
            );
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
            if (weapon.IsReloading)
                return;

            if (firstSkillActionInfo.Skill && PlayerInputInfo.pressedFirstSkill)
                firstSkillActionInfo.SkillAction.Activate(EJob.DEALER);

            else if (secondSkillActionInfo.Skill && PlayerInputInfo.pressedSecondSkill)
                secondSkillActionInfo.SkillAction.Activate(EJob.DEALER);
        }

        public void SetIsAiming(bool state)
        {
            IsAiming = state;
        }

        public void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            throw new System.NotImplementedException();
        }

        public bool IsGrounded
        {
            get
            {
                return Physics.CheckSphere(
                    PlayerTransform.TransformPoint(groundCheckOffset),
                    groundCheckRadius,
                    groundLayer
                );
            }
        }

        public bool IsUsingSkill
        {
            get
            {
                if (firstSkillActionInfo.SkillAction != null && firstSkillActionInfo.SkillAction.IsUsingSkill)
                    return true;

                if (secondSkillActionInfo.SkillAction != null && secondSkillActionInfo.SkillAction.IsUsingSkill)
                    return true;

                return false;
            }
        }
        
        public bool IsAiming { get; private set; }
        public bool IsShooting => PlayerInputInfo.holdLeftMouseButton;
        public bool IsMoving => moveInput.magnitude > 0;

        #region GIZMO
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
