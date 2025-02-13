using UnityEngine;
using EverScord.Pool;

namespace EverScord.Weapons
{
    public class Bullet : MonoBehaviour, IPoolable
    {
        private const float COLLISION_STEP = 0.5f;
        
        [field: SerializeField] public TrailRenderer TracerEffect   { get; private set; }
        public BulletInfo BulletInfo                                { get; private set; }
        public Vector3 InitialPosition                              { get; private set; }
        public Vector3 InitialVelocity                              { get; private set; }
        public float Lifetime                                       { get; private set; }
        public bool IsDestroyed                                     { get; private set; }
        public int ViewID                                           { get; private set; }

        private int bulletID = -1;
        public int BulletID => bulletID;

        public void Init(Vector3 position, Vector3 velocity, BulletInfo bulletInfo, int viewID)
        {
            InitialPosition = position;
            InitialVelocity = velocity;

            BulletInfo = bulletInfo;
            TracerEffect.material = bulletInfo.TracerMaterial;
            TracerEffect.colorGradient = bulletInfo.TracerGradient;

            ViewID = viewID;

            if (bulletID == -1)
                bulletID = BulletControl.GetNextBulletID();

            Lifetime = 0f;
            IsDestroyed = false;

            TracerEffect.AddPosition(position);
            SetTracerEffectPosition(position);
        }

        public void SetBulletID(int bulletID)
        {
            this.bulletID = bulletID;
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

        public void CheckCollision(Weapon sourceWeapon, Vector3 startPoint, Vector3 endPoint)
        {
            RaycastHit hit = new RaycastHit();
            Vector3 direction = endPoint - startPoint;
            float totalDistance = direction.magnitude;

            direction.Normalize();

            for (float distance = 0f; distance <= totalDistance; distance += COLLISION_STEP)
            {
                Vector3 currentPoint = startPoint + direction * distance;
                Vector3 currentScreenPoint = sourceWeapon.ShooterCam.WorldToScreenPoint(currentPoint);
                
                bool isWithinScreen = Screen.safeArea.Contains(currentScreenPoint);

                if (isWithinScreen)
                {
                    Ray ray = sourceWeapon.ShooterCam.ScreenPointToRay(currentScreenPoint);

                    if (!Physics.Raycast(ray, out hit, 50f, sourceWeapon.ShootableLayer))
                        continue;

                    GameManager.Instance.BulletsControl.BulletHitEffect(hit.point, -direction);

                    if (hit.transform.gameObject.layer == GameManager.EnemyLayerNumber)
                    {
                        IEnemy monster = hit.transform.GetComponent<IEnemy>();
                        GameManager.Instance.EnemyHitsControl.ApplyDamageToEnemy(sourceWeapon.Damage, monster);
                    }

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

        public void SetGameObject(bool state)
        {
            gameObject.SetActive(state);
        }
    }
}
