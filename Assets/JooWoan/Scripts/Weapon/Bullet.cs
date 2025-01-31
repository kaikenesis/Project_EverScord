using Photon.Pun.Demo.Asteroids;
using UnityEngine;

namespace EverScord.Weapons
{
    public class Bullet
    {
        private const float COLLISION_STEP = 0.5f;
        public TrailRenderer TracerEffect   { get; private set; }
        public Vector3 InitialPosition      { get; private set; }
        public Vector3 InitialVelocity      { get; private set; }
        public float Lifetime               { get; private set; }
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

        private void SetTracerEffectPosition(Vector3 position)
        {
            TracerEffect.transform.position = position;
        }

        public void DestroyTracerEffect()
        {
            if (TracerEffect == null)
                return;

            Object.Destroy(TracerEffect.gameObject);
        }

        public bool ShouldBeDestroyed(float weaponRange)
        {
            if (IsDestroyed)
                return true;

            if (!TracerEffect)
                return true;

            return Vector3.Distance(GetPosition(), InitialPosition) > weaponRange;
        }

        public void CheckCollision(BulletCollisionParam param)
        {
            RaycastHit hit = new RaycastHit();
            Vector3 direction = param.EndPoint - param.StartPoint;
            float totalDistance = direction.magnitude;

            direction.Normalize();

            for (float distance = 0f; distance <= totalDistance; distance += COLLISION_STEP)
            {
                Vector3 currentPoint = param.StartPoint + direction * distance;
                Vector3 currentScreenPoint = param.PlayerCam.WorldToScreenPoint(currentPoint);
                
                bool isWithinScreen = Screen.safeArea.Contains(currentScreenPoint);

                if (isWithinScreen)
                {
                    Ray ray = param.PlayerCam.ScreenPointToRay(currentScreenPoint);

                    if (!Physics.Raycast(ray, out hit, 50f, param.ShootableLayer))
                        continue;

                    param.HitEffect.transform.position = hit.point;
                    param.HitEffect.transform.forward = -direction;
                    param.HitEffect.Emit(param.HitEffectCount);

                    SetTracerEffectPosition(currentPoint);
                }

                IsDestroyed = true;
                return;
            }

            SetTracerEffectPosition(param.EndPoint);
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

    public class BulletCollisionParam
    {
        public Vector3 StartPoint;
        public Vector3 EndPoint;
        public LayerMask ShootableLayer;
        public Camera PlayerCam;
        public ParticleSystem HitEffect;
        public int HitEffectCount;
    }
}
