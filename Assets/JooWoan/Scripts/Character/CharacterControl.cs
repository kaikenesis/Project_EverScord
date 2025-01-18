using UnityEngine;
using EverScord.Weapons;

namespace EverScord.Character
{
    public class CharacterControl : MonoBehaviour
    {
        [Header("Character")]
        [SerializeField] private float speed, gravity;

        [Header("Ground Check")]
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private float groundCheckRadius;
        [SerializeField] private Vector3 groundCheckOffset;

        [Header("Animation")]
        [SerializeField] private Animator anim;
        [SerializeField] private float transitionDampTime;
        [SerializeField] private float smoothRotation;
        [field: SerializeField] public float ShootStanceDuration { get; private set; }

        [Header("Weapon")]
        [SerializeField] private GameObject weaponPrefab;
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private float coolDown;

        public CharacterAnimation AnimationControl              { get; private set; }
        public InputInfo PlayerInputInfo                        { get; private set; }

        private CharacterController controller;
        private Weapon weapon;
        private Camera mainCam;

        private Vector3 movement, lookPosition, moveInput, moveDir;
        private Quaternion lookRotation;
        private float fallSpeed;


        void Awake()
        {
            AnimationControl = new CharacterAnimation(
                anim,
                smoothRotation,
                transitionDampTime
            );

            weapon = new Weapon(bulletPrefab, coolDown);
            controller = GetComponent<CharacterController>();

            // Unity docs: Set skinwidth 10% of the Radius
            controller.skinWidth = controller.radius * 0.1f;

            mainCam = Camera.main;
        }

        void Update()
        {
            SetInput();
            SetMovingDirection();

            AnimationControl.AnimateMovement(this, moveDir);

            weapon.CooldownTimer();
            weapon.Shoot(this);

            ApplyGravity();
            Move();
            Turn();
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

            moveDir = transform.InverseTransformDirection(moveInput);
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

        private void Turn()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, groundLayer))
            {
                lookPosition = hit.point;
                lookPosition.y = transform.position.y;
                lookRotation = Quaternion.LookRotation(lookPosition - transform.position);
            }

            transform.rotation = Quaternion.Lerp(
                transform.rotation,
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
                    transform.TransformPoint(groundCheckOffset),
                    groundCheckRadius,
                    groundLayer
                );
            }
        }

        public bool IsShooting
        {
            get
            {
                return PlayerInputInfo.holdLeftMouseButton;
            }
        }

        public bool IsAiming { get; private set; }
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
