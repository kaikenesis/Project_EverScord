using System.Collections;
using UnityEngine;
using EverScord.Weapons;
using EverScord.Pool;

namespace EverScord.Effects
{
    public class SmokeTrail : MonoBehaviour, IPoolable
    {
        [SerializeField] private ParticleSystem effect;
        [SerializeField] private float smokeFadeTime;
        private Bullet bullet;

        public void Init(Bullet bullet)
        {
            this.bullet = bullet;
            transform.position = bullet.GetPosition();
            effect.Play();
        }

        void Update()
        {
            FollowBullet();
        }

        private void FollowBullet()
        {
            if (bullet == null || bullet.IsDestroyed)
            {
                effect.Stop();
                StartCoroutine(DestroySmokeTrail());
                return;
            }

            transform.position = bullet.GetPosition();
        }

        private IEnumerator DestroySmokeTrail()
        {
            yield return new WaitForSeconds(smokeFadeTime);
            
            Weapon weapon = GameManager.Instance.PlayerDict[bullet.ViewID].PlayerWeapon;
            ResourceManager.Instance.ReturnToPool(this, AssetReferenceManager.BulletSmoke_ID);
        }

        public void SetGameObject(bool state)
        {
            gameObject.SetActive(state);
        }
    }
}
