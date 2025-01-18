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
        [field: SerializeField] public float shootStanceDuration { get; private set; }

        [Header("Weapon")]
        [SerializeField] private GameObject weaponPrefab;
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private float coolDown;

        public CharacterAnimation characterAnimation    { get; private set; }
        public InputInfo playerInputInfo                { get; private set; }

        private CharacterController characterControl;
        private Weapon weapon;
        private Camera mainCam;

        private Vector3 movement, lookPosition, moveInput, moveDir;
        private Quaternion lookRotation;
        private float fallSpeed;

        void Awake()
        {
            characterAnimation = new CharacterAnimation(
                anim,
                smoothRotation,
                transitionDampTime
            );

            weapon = new Weapon(bulletPrefab, coolDown);
            characterControl = GetComponent<CharacterController>();

            // Unity docs: Set skinwidth 10% of the Radius
            characterControl.skinWidth = characterControl.radius * 0.1f;

            mainCam = Camera.main;
        }

        void Update()
        {
            SetInput();
            SetMovingDirection();

            characterAnimation.AnimateMovement(moveDir);

            weapon.CooldownTimer();
            weapon.Shoot(this);

            ApplyGravity();
            Move();
            Turn();
        }

        private void SetInput()
        {
            playerInputInfo = InputControl.ReceiveInput();
            playerInputInfo = InputControl.GetCameraRelativeInput(playerInputInfo, mainCam);

            moveInput = playerInputInfo.cameraRelativeInput;
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
            
            characterControl.Move(velocity * Time.deltaTime);
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

            transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * smoothRotation);
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
                return playerInputInfo.holdLeftMouseButton;
            }
        }

        public bool IsMoving => characterControl.velocity.magnitude > 0;

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
