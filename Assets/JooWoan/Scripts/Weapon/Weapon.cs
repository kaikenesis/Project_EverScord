using EverScord.Character;
using UnityEngine;

namespace EverScord.Weapons
{
    public class Weapon
    {
        private GameObject bulletPrefab;
        private float cooldown, elapsedTime;
        private bool isCooldown => elapsedTime < cooldown;

        public Weapon(GameObject bulletPrefab, float cooldown)
        {
            this.bulletPrefab = bulletPrefab;
            this.cooldown = cooldown;
        }

        public void CooldownTimer()
        {
            elapsedTime += Time.deltaTime;
        }

        public void ResetCooldownTimer()
        {
            elapsedTime = cooldown;
        }

        public void Shoot(CharacterControl shooter)
        {
            float cooldownOvertime = elapsedTime - cooldown;

            if (shooter.IsAiming && (cooldownOvertime > shooter.ShootStanceDuration))
            {
                shooter.SetIsAiming(false);
                shooter.AnimationControl.SetAimRig(shooter);
                shooter.AnimationControl.Play(ConstStrings.ANIMATION_NED_SHOOTEND);
                return;
            }

            if (isCooldown || !shooter.IsShooting)
                return;
            
            elapsedTime = 0f;
            shooter.SetIsAiming(true);
            shooter.AnimationControl.SetAimRig(shooter);

            // GameObject bullet = PoolManager.GetObject(bulletPrefab.name);
            // bullet.transform.position = shootTransform.position;

            shooter.AnimationControl.Play(ConstStrings.ANIMATION_NED_SHOOT);
        }
    }
}
