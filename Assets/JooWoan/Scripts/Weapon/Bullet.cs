using UnityEngine;

namespace EverScord.Weapons
{
    public class Bullet
    {
        private const float COLLISION_STEP = 0.5f;
        public Weapon SourceWeapon          { get; private set; }
        public TrailRenderer TracerEffect   { get; private set; }
        public Vector3 InitialPosition      { get; private set; }
        public Vector3 InitialVelocity      { get; private set; }
        public float Lifetime               { get; private set; }
        public bool IsDestroyed             { get; private set; }

        public void Init(Weapon sourceWeapon, Vector3 position, Vector3 velocity)
        {
            SourceWeapon = sourceWeapon;
            InitialPosition = position;
            InitialVelocity = velocity;

            Lifetime = 0f;
            IsDestroyed = false;

            TracerEffect.AddPosition(position);
            SetTracerEffectPosition(position);
        }

        public void SetLifetime(float lifeTime)
        {
            Lifetime = lifeTime;
        }

        public void SetTracerEffect(TrailRenderer effect)
        {
            TracerEffect = effect;
        }

        public void SetTracerEffectPosition(Vector3 position)
        {
            TracerEffect.transform.position = position;
        }

        public bool ShouldBeDestroyed(float weaponRange)
        {
            if (IsDestroyed)
                return true;

            if (!TracerEffect)
                return true;

            return Vector3.Distance(GetPosition(), InitialPosition) > weaponRange;
        }

        public void CheckCollision(Vector3 startPoint, Vector3 endPoint)
        {
            RaycastHit hit = new RaycastHit();
            Vector3 direction = endPoint - startPoint;
            float totalDistance = direction.magnitude;

            direction.Normalize();

            for (float distance = 0f; distance <= totalDistance; distance += COLLISION_STEP)
            {
                Vector3 currentPoint = startPoint + direction * distance;
                Vector3 currentScreenPoint = SourceWeapon.ShooterCam.WorldToScreenPoint(currentPoint);
                
                bool isWithinScreen = Screen.safeArea.Contains(currentScreenPoint);

                if (isWithinScreen)
                {
                    Ray ray = SourceWeapon.ShooterCam.ScreenPointToRay(currentScreenPoint);

                    if (!Physics.Raycast(ray, out hit, 50f, SourceWeapon.ShootableLayer))
                        continue;

                    SourceWeapon.HitEffect.transform.position = hit.point;
                    SourceWeapon.HitEffect.transform.forward  = -direction;
                    SourceWeapon.HitEffect.Emit(SourceWeapon.HitEffectCount);

                    SetTracerEffectPosition(currentPoint);
                }

                SetIsDestroyed(true);
                return;
            }

            SetTracerEffectPosition(endPoint);
        }

        public void SetIsDestroyed(bool state)
        {
            IsDestroyed = state;
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
