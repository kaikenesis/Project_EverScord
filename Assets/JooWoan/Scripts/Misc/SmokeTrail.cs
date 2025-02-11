using System.Collections;
using UnityEngine;
using EverScord.Weapons;

namespace EverScord
{
    public class SmokeTrail : MonoBehaviour
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
            ResourceManager.instance.ReturnToPool(gameObject, weapon.SmokeAssetReference.AssetGUID);
        }
    }
}
