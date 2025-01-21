using UnityEngine;

namespace EverScord.Weapons
{
    public class Bullet
    {
        public float Lifetime               { get; private set; }
        public Vector3 InitialPosition      { get; private set; }
        public Vector3 InitialVelocity      { get; private set; }
        public TrailRenderer TracerEffect   { get; private set; }

        public Bullet(Vector3 position, Vector3 velocity, TrailRenderer effect)
        {
            InitialPosition = position;
            InitialVelocity = velocity;
            TracerEffect    = effect;
            Lifetime = 0f;

            TracerEffect.AddPosition(position);
        }

        public void SetLifetime(float lifeTime)
        {
            Lifetime = lifeTime;
        }

        public void SetTracerEffectPosition(Vector3 position)
        {
            TracerEffect.transform.position = position;
        }

        #region Projectile Motion Equation
        /*
            s(t) = h0 + (v0 * x) - (0.5 * g * x^2)
            
            h0: initial position
            v0: velocity
            x : time
            g : gravity
        */
        #endregion
        public Vector3 GetPosition()
        {
            // Exclude bullet drop
            return InitialPosition + InitialVelocity * Lifetime;
        }
    }
}
