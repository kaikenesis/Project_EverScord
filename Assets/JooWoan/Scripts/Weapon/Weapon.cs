using EverScord.Character;
using UnityEngine;

namespace EverScord.Weapons
{
    public class Weapon
    {
        private GameObject bulletPrefab;
        private float cooldown, elapsedTime;

        private bool flag = false;
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

            if (flag && (cooldownOvertime > shooter.shootStanceDuration))
            {
                flag = false;
                shooter.characterAnimation.AdjustPosture(false);
                shooter.characterAnimation.Play("Ned_ShootEnd");
                return;
            }

            if (isCooldown || !shooter.IsShooting)
                return;
            
            elapsedTime = 0f;
            flag = true;

            // GameObject bullet = PoolManager.GetObject(bulletPrefab.name);
            // bullet.transform.position = shootTransform.position;

            shooter.characterAnimation.AdjustPosture(true);
            shooter.characterAnimation.Play("Ned_Shoot");
        }
    }
}
