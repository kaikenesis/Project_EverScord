using UnityEngine;
using EverScord.Weapons;
using UnityEngine.Animations.Rigging;

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
        [field: SerializeField] public MultiAimConstraint Aim           { get; private set; }
        [field: SerializeField] public TwoBoneIKConstraint LeftHandIK   { get; private set; }
        [SerializeField] private float transitionDampTime;
        [SerializeField] private float rotateAngle;
        [SerializeField] private float smoothRotation;
        [field: SerializeField] public float ShootStanceDuration        { get; private set; }

        [Header("Weapon")]
        [SerializeField] private GameObject weaponPrefab;
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private Transform aimPoint;
        [SerializeField] private float aimSensitivity;
        [SerializeField] private float minAimDistance;
        [SerializeField] private float coolDown;

        public CharacterAnimation AnimationControl                      { get; private set; }
        public InputInfo PlayerInputInfo                                { get; private set; }
        
        private Weapon weapon;
        private Camera mainCam;
        private CharacterController controller;
        private Vector3 movement, lookPosition, lookDir, moveInput, moveDir;
        private Transform characterTransform, characterSpine, weaponTransform;
        private float fallSpeed;

        void Awake()
        {
            AnimationControl = new CharacterAnimation(
                anim,
                smoothRotation,
                transitionDampTime
            );

            mainCam = Camera.main;

            weapon = new Weapon(bulletPrefab, coolDown);
            weaponTransform = weaponPrefab.transform;

            aimPoint = Instantiate(aimPoint).transform;
            InitRig();

            // Unity docs: Set skinwidth 10% of the Radius
            controller = GetComponent<CharacterController>();
            controller.skinWidth = controller.radius * 0.1f;

            characterTransform = transform;

            if (bodyAim.data.sourceObjects.Count > 0)
                characterSpine = bodyAim.data.constrainedObject;
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

            weapon.CooldownTimer();
            weapon.Shoot(this);

            TrackAim();
            RotateBody();
        }

        private void InitRig()
        {
            MultiAimConstraint[] constraints = { Aim, bodyAim, headAim };

            for (int i = 0; i < constraints.Length; i++)
            {
                var data = constraints[i].data.sourceObjects;
                data.Clear();
                data.Add(new WeightedTransform(aimPoint, 1));
                constraints[i].data.sourceObjects = data;
            }
            
            rigBuilder.Build();
        }

        private void SetInput()
        {
            PlayerInputInfo = InputControl.ReceiveInput();
            PlayerInputInfo = InputControl.GetCameraRelativeInput(PlayerInputInfo, mainCam);

            moveInput = PlayerInputInfo.cameraRelativeInput;
        }

        private void SetMovingDirection()
        {
            if (moveInput.magnitude > 1f)
                moveInput.Normalize();

            moveDir = characterTransform.InverseTransformDirection(moveInput);
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
            movement = new Vector3(moveInput.x, 0, moveInput.z);

            Vector3 velocity = movement * speed;
            velocity.y = fallSpeed;

            controller.Move(velocity * Time.deltaTime);
        }

        private void TrackAim()
        {
            Ray ray = mainCam.ScreenPointToRay(PlayerInputInfo.mousePosition);

            if (!Physics.Raycast(ray, out RaycastHit hit, groundLayer))
                return;
            
            lookPosition = hit.point;
            lookPosition.y = characterTransform.position.y;

            lookDir = lookPosition - characterTransform.position;
            float distance = lookDir.magnitude;

            lookDir.Normalize();

            if (distance < 0.5f)
                return;

            if (distance < minAimDistance)
                lookPosition = characterTransform.position + lookDir * minAimDistance;

            lookPosition.y = weaponTransform.position.y;
            
            aimPoint.position = Vector3.Lerp(
                aimPoint.position,
                lookPosition,
                Time.deltaTime * aimSensitivity
            );
        }

        private void RotateBody()
        {
            if (!characterSpine)
            {
                Debug.LogWarning("Body aim constrained object not initialized");
                return;
            }
            
            float angle = Vector3.Angle(lookDir, characterTransform.forward);

            if (angle <= rotateAngle)
            {
                AnimationControl.Rotate(false);
                return;
            }
            
            AnimationControl.Rotate(!IsMoving);
            Quaternion lookRotation = Quaternion.LookRotation(lookDir);

            characterTransform.rotation = Quaternion.Slerp(
                characterTransform.rotation,
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
                    characterTransform.TransformPoint(groundCheckOffset),
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
