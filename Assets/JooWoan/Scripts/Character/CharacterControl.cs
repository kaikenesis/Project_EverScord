using UnityEngine;

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

        [Header("Weapon")]
        [SerializeField] private GameObject weapon;

        private CharacterAnimation characterAnimation;
        private CharacterController characterControl;

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

            ApplyGravity();
            Move();
            Turn();
        }

        private void SetInput()
        {
            moveInput = InputControl.ConvertRelativeInput(
                InputControl.ReceiveInput(),
                mainCam
            );
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
