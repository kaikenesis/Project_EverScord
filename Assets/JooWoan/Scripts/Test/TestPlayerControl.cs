using UnityEngine;

namespace EverScord.Character
{
    public class TestPlayerControl : MonoBehaviour
    {
        [SerializeField] private float speed, gravity;

        [Header("Ground Check")]
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private float groundCheckRadius;
        [SerializeField] private Vector3 groundCheckOffset;

        private CharacterController characterControl;
        private Vector3 movement, lookPosition;
        private float fallSpeed;


        void Awake()
        {
            characterControl = GetComponent<CharacterController>();

            // Unity docs: Set skinwidth 10% of the Radius
            characterControl.skinWidth = characterControl.radius * 0.1f;
        }

        void Update()
        {
            ReceiveInput();
            ApplyGravity();
            Move();
            Turn();
        }

        private void ReceiveInput()
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            movement = new Vector3(horizontal, 0, vertical).normalized;
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
            Vector3 velocity = movement * speed;
            velocity.y = fallSpeed;
            
            characterControl.Move(velocity * Time.deltaTime);
        }

        private void Turn()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
                lookPosition = hit.point;

            lookPosition.y = transform.position.y;
            transform.LookAt(lookPosition, Vector3.up);
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
