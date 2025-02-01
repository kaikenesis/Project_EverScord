using UnityEngine;

namespace EverScord.Character
{
    public class CharacterPhysics
    {
        private Vector3 impactForce;
        private float gravity, mass;
        public float FallSpeed { get; private set; }

        public CharacterPhysics(float gravity, float mass)
        {
            this.gravity = gravity;
            this.mass = mass;
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

        public void AddImpact(Vector3 dir, float force)
        {
            //impactForce += dir.normalized * force
        }
    }
}
