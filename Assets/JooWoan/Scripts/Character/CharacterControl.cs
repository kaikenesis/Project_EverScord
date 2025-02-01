using UnityEngine;
using EverScord.Weapons;
using UnityEngine.Animations.Rigging;
using Photon.Pun;
using EverScord.UI;
using EverScord.GameCamera;

namespace EverScord.Character
{
    public class CharacterControl : MonoBehaviour
    {
        [Header("Character")]
        [SerializeField] private float speed;
        [SerializeField] private float gravity;

        [Header("Ground Check")]
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private float groundCheckRadius;
        [SerializeField] private Vector3 groundCheckOffset;

        [Header("Animation")]
        [SerializeField] private Animator anim;
        [SerializeField] private AnimationInfo info;
        [SerializeField] private CharacterRigControl rigLayerPrefab;
        [SerializeField] private float transitionDampTime;
        [SerializeField] private float rotateAngle;
        [SerializeField] private float smoothRotation;
        [field: SerializeField] public float ShootStanceDuration        { get; private set; }
        public CharacterRigControl RigControl                           { get; private set; }

        [Header("Weapon")]
        [SerializeField] private GameObject playerWeapon;
        private Weapon weapon;
        public Weapon PlayerWeapon => weapon;

        [Header("UI")]
        [SerializeField] private PlayerUI uiPrefab;
        public PlayerUI PlayerUIControl                                 { get; private set; }
        private Transform uiHub;

        public CharacterAnimation AnimationControl                      { get; private set; }
        public CharacterCamera CameraControl                            { get; private set; }
        public Transform PlayerTransform                                { get; private set; }
        public InputInfo PlayerInputInfo                                { get; private set; }
        public Vector3 AimPosition                                      { get; private set; }

        private CharacterController controller;
        private PhotonView photonView;
        private Vector3 movement, lookPosition, lookDir, moveInput, moveDir;
        private float fallSpeed;

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

            // Unity docs: Set skinwidth 10% of the Radius
            controller.skinWidth = controller.radius * 0.1f;

            weapon.Init(PlayerUIControl.SetAmmoText);
            InitRig();

            AnimationControl = new CharacterAnimation(anim, info, transitionDampTime);

            RigControl.SetAimWeight(false);
            CameraControl.Init(PlayerTransform, Camera.main);
            PlayerUIControl.Init(this);
        }

        void OnApplicationFocus(bool focus)
        {
            // Cursor.visible = !focus;
        }

        void Update()
        {
            SetInput();
            SetMovingDirection();

            ApplyGravity();
            Move();

            AnimationControl.AnimateMovement(this, moveDir);

            TrackAim();
            RotateBody();

            weapon.CooldownTimer();
            weapon.TryReload(this);
            weapon.Shoot(this);
            weapon.UpdateBullets(this, Time.deltaTime);
        }

        private void InitRig()
        {
            RigControl.Init(anim.transform, GetComponent<Animator>(), weapon);

            MultiAimConstraint[] constraints = { RigControl.Aim, RigControl.BodyAim, RigControl.HeadAim };

            for (int i = 0; i < constraints.Length; i++)
            {
                var data = constraints[i].data.sourceObjects;
                data.Clear();
                data.Add(new WeightedTransform(weapon.AimPoint, 1));
                constraints[i].data.sourceObjects = data;
            }

            RigControl.Builder.Build();
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
        
        private void ApplyGravity()
        {
            if (IsGrounded)
            {
                fallSpeed = -0.5f;
                return;
            }

            fallSpeed += gravity * Time.deltaTime;
        }

        private void Move()
        {
            if (photonView.IsMine == false)
                return;

            movement = new Vector3(moveInput.x, 0, moveInput.z);

            Vector3 velocity = movement * speed;
            velocity.y = fallSpeed;

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

            if (angle <= rotateAngle)
            {
                AnimationControl.Rotate(false);
                return;
            }
            
            AnimationControl.Rotate(!IsMoving);
            Quaternion lookRotation = Quaternion.LookRotation(lookDir);

            PlayerTransform.rotation = Quaternion.Lerp(
                PlayerTransform.rotation,
                lookRotation,
                Time.deltaTime * smoothRotation
            ).normalized;
        }

        public void SetIsAiming(bool state)
        {
            IsAiming = state;
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
