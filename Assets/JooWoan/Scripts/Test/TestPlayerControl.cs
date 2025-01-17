using UnityEngine;

namespace EverScord.Character
{
    public class TestPlayerControl : MonoBehaviour
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

        private CharacterController characterControl;
        private Vector3 movement, lookPosition;
        private Quaternion lookRotation;
        private float fallSpeed, horizontalInput, verticalInput;

        private Camera mainCam;
        private Vector3 camForward, camRight, moveInput, convertedInput;

        void Awake()
        {
            characterControl = GetComponent<CharacterController>();
            mainCam = Camera.main;

            // Unity docs: Set skinwidth 10% of the Radius
            characterControl.skinWidth = characterControl.radius * 0.1f;
        }

        void Update()
        {
            ReceiveInput();
            ConvertInput();

            AnimateMovement();

            ApplyGravity();
            Move();
            Turn();
        }

        private void ReceiveInput()
        {
            horizontalInput = Input.GetAxisRaw("Horizontal");
            verticalInput   = Input.GetAxisRaw("Vertical");

            camForward = Vector3.Scale(mainCam.transform.forward, new Vector3(1, 0, 1)).normalized;
            camRight   = Vector3.Scale(mainCam.transform.right,   new Vector3(1, 0, 1)).normalized;
            
            moveInput  = horizontalInput * camRight + verticalInput * camForward;
        }

        private void ConvertInput()
        {
            if (moveInput.magnitude > 1f)
                moveInput.Normalize();
            
            convertedInput = transform.InverseTransformDirection(moveInput);
        }

        private void AnimateMovement()
        {
            anim.SetFloat("Horizontal", convertedInput.x, transitionDampTime, Time.deltaTime);
            anim.SetFloat("Vertical",   convertedInput.z, transitionDampTime, Time.deltaTime);
        }
        
        private void ApplyGravity()
        {
            if (IsGrounded)
                fallSpeed = -0.5f;
            else
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
