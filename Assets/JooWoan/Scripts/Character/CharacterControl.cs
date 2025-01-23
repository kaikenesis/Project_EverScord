using UnityEngine;
using EverScord.Weapons;
using UnityEngine.Animations.Rigging;
using Photon.Pun;
using EverScord.UI;
using Unity.VisualScripting;

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
        [SerializeField] private RigBuilder rigBuilder;
        [SerializeField] private MultiAimConstraint bodyAim;
        [SerializeField] private MultiAimConstraint headAim;
        [SerializeField] private MultiAimConstraint aim;
        [SerializeField] private TwoBoneIKConstraint leftHandIK;
        [SerializeField] private float shootStanceDuration;
        [SerializeField] private float transitionDampTime;
        [SerializeField] private float rotateAngle;
        [SerializeField] private float smoothRotation;
        public float ShootStanceDuration => shootStanceDuration;

        [Header("Weapon")]
        [SerializeField] private GameObject weaponPrefab;
        private Weapon weapon;
        public Weapon PlayerWeapon => weapon;

        [Header("UI")]
        [SerializeField] private GameObject playerUIPrefab;
        private PlayerUI playerUI;
        private Transform uiCanvas;

        public CharacterAnimation AnimationControl                      { get; private set; }
        public Transform CharacterTransform                             { get; private set; }
        public InputInfo PlayerInputInfo                                { get; private set; }
        public Vector3 AimPosition                                      { get; private set; }
        public Camera MainCam                                           { get; private set; }

        private CharacterController controller;
        private PhotonView photonView;
        private Vector3 movement, lookPosition, lookDir, moveInput, moveDir;
        private float fallSpeed;

        void Awake()
        {
            photonView = GetComponent<PhotonView>();

            AnimationControl = new CharacterAnimation(
                anim,
                aim,
                leftHandIK,
                smoothRotation,
                transitionDampTime
            );

            MainCam = Camera.main;
            CharacterTransform = transform;

            // Unity docs: Set skinwidth 10% of the Radius
            controller = GetComponent<CharacterController>();
            controller.skinWidth = controller.radius * 0.1f;

            uiCanvas = GameObject.FindGameObjectWithTag(ConstStrings.TAG_PLAYERUI).transform;
            playerUI = Instantiate(playerUIPrefab, uiCanvas).GetComponent<PlayerUI>();

            weapon = weaponPrefab.GetComponent<Weapon>();
            weapon.Init(playerUI.SetAmmoText);
            InitRig();

            playerUI.Init(this);
        }

        void OnApplicationFocus(bool focus)
        {
            //Cursor.visible = !focus;
        }

        void Start()
        {
            AnimationControl.SetAimRig(this);
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
            weapon.Shoot(this);
            weapon.UpdateBullets(this, Time.deltaTime);
        }

        private void InitRig()
        {
            MultiAimConstraint[] constraints = { aim, bodyAim, headAim };

            for (int i = 0; i < constraints.Length; i++)
            {
                var data = constraints[i].data.sourceObjects;
                data.Clear();
                data.Add(new WeightedTransform(weapon.AimPoint, 1));
                constraints[i].data.sourceObjects = data;
            }
            
            rigBuilder.Build();
        }

        private void SetInput()
        {
            PlayerInputInfo = InputControl.ReceiveInput();
            PlayerInputInfo = InputControl.GetCameraRelativeInput(PlayerInputInfo, MainCam);

            moveInput = PlayerInputInfo.cameraRelativeInput;
        }

        private void SetMovingDirection()
        {
            if (moveInput.magnitude > 1f)
                moveInput.Normalize();

            moveDir = CharacterTransform.InverseTransformDirection(moveInput);
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
            Ray ray = MainCam.ScreenPointToRay(PlayerInputInfo.mousePosition);

            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
                return;

            AimPosition = hit.point;
            lookPosition = hit.point;
            lookPosition.y = CharacterTransform.position.y;

            lookDir = lookPosition - CharacterTransform.position;
            float distance = lookDir.magnitude;

            lookDir.Normalize();

            if (distance < 0.5f)
                return;

            if (distance < weapon.MinAimDistance)
                lookPosition = CharacterTransform.position + lookDir * weapon.MinAimDistance;

            lookPosition.y = weapon.GunPoint.position.y;

            weapon.AimPoint.position = Vector3.Lerp(
                weapon.AimPoint.position,
                lookPosition,
                Time.deltaTime * weapon.AimSensitivity
            );
        }

        private void RotateBody()
        {
            float angle = Vector3.Angle(lookDir, CharacterTransform.forward);

            if (angle <= rotateAngle)
            {
                AnimationControl.Rotate(false);
                return;
            }
            
            AnimationControl.Rotate(!IsMoving);
            Quaternion lookRotation = Quaternion.LookRotation(lookDir);

            CharacterTransform.rotation = Quaternion.Lerp(
                CharacterTransform.rotation,
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
                    CharacterTransform.TransformPoint(groundCheckOffset),
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
