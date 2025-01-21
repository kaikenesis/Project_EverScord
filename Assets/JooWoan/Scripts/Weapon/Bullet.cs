using Photon.Pun.Demo.Asteroids;
using UnityEngine;

namespace EverScord.Weapons
{
    public class Bullet
    {
        public float Lifetime               { get; private set; }
        public Vector3 InitialPosition      { get; private set; }
        public Vector3 InitialVelocity      { get; private set; }
        public TrailRenderer TracerEffect   { get; private set; }
        public bool IsDestroyed             { get; private set; }

        public Bullet(Vector3 position, Vector3 velocity, TrailRenderer effect)
        {
            InitialPosition = position;
            InitialVelocity = velocity;
            TracerEffect    = effect;

            Lifetime = 0f;
            IsDestroyed = false;

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

        public void SetIsDestroyed(bool state)
        {
            IsDestroyed = state;
        }

        public bool ShouldBeDestroyed(float weaponRange)
        {
            if (IsDestroyed)
                return true;

            if (!TracerEffect)
                return true;

            return Vector3.Distance(GetPosition(), InitialPosition) > weaponRange;
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
