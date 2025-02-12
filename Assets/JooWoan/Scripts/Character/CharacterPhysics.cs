using UnityEngine;

namespace EverScord.Character
{
    public class CharacterPhysics
    {
        public float FallSpeed { get; private set; }
        private float gravity, mass;
        private Vector3 impact;
        public Vector3 ImpactVelocity => impact;

        private const float MOVE_IMPACT_SIZE = 0.04f;
        private const float LERP_IMPACT_SPEED = 5f;
        public bool IsImpactAdded => impact.sqrMagnitude > MOVE_IMPACT_SIZE;

        public CharacterPhysics(float gravity, float mass)
        {
            this.gravity = gravity;
            this.mass = Mathf.Max(mass, 1f);
        }

        public void SetFallSpeed(float fallSpeed)
        {
            FallSpeed = fallSpeed;
        }

        public void ApplyGravity(CharacterControl character)
        {
            if (character.IsGrounded)
            {
                FallSpeed = -0.5f;
                return;
            }

            FallSpeed += gravity * Time.deltaTime;
        }

        public void AddImpact(Vector3 direction, float force)
        {
            direction.Normalize();
            impact += direction * force / mass;
        }

        public void ApplyImpact(CharacterControl character)
        {
            if (IsImpactAdded)
                character.Controller.Move(impact * Time.deltaTime);
            
            impact = Vector3.Lerp(impact, Vector3.zero, Time.deltaTime * LERP_IMPACT_SPEED);
        }
    }
}
