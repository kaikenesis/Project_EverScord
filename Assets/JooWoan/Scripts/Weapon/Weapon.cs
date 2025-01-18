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
                shooter.AnimationControl.AdjustPosture(false);
                shooter.AnimationControl.Play("Ned_ShootEnd");
                return;
            }

            if (isCooldown || !shooter.IsShooting)
                return;
            
            elapsedTime = 0f;
            shooter.SetIsAiming(true);

            // GameObject bullet = PoolManager.GetObject(bulletPrefab.name);
            // bullet.transform.position = shootTransform.position;

            shooter.AnimationControl.AdjustPosture(true);
            shooter.AnimationControl.Play("Ned_Shoot");
        }
    }
}
