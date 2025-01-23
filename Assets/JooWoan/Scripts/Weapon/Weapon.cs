using UnityEngine;
using EverScord.Character;
using System.Collections.Generic;

namespace EverScord.Weapons
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] private ParticleSystem shotEffect, hitEffect;
        [SerializeField] private TrailRenderer tracerEffect;
        [field: SerializeField] public Transform GunPoint           { get; private set; }
        [field: SerializeField] public Transform AimPoint           { get; private set; }
        [field: SerializeField] public LayerMask ShootableLayer     { get; private set; }
        [field: SerializeField] public float AimSensitivity         { get; private set; }
        [field: SerializeField] public float MinAimDistance         { get; private set; }
        [field: SerializeField] public float Cooldown               { get; private set; }

        [SerializeField] private float weaponRange;
        [SerializeField] public float bulletSpeed;
        [SerializeField] private int hitEffectCount;

        private LinkedList<Bullet> bullets = new();
        private BulletCollisionParam bulletCollisionParam = new();
        private float elapsedTime;
        private bool isCooldown => elapsedTime < Cooldown;

        public void Init()
        {
            AimPoint = Instantiate(AimPoint).transform;
        }

        public void CooldownTimer()
        {
            elapsedTime += Time.deltaTime;
        }

        public void ResetCooldownTimer()
        {
            elapsedTime = Cooldown;
        }

        public void Shoot(CharacterControl shooter)
        {
            float cooldownOvertime = elapsedTime - Cooldown;

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
            shotEffect.Emit(1);

            shooter.SetIsAiming(true);
            shooter.AnimationControl.SetAimRig(shooter);
            shooter.AnimationControl.Play(ConstStrings.ANIMATION_NED_SHOOT);

            FireBullet();
        }

        private void FireBullet()
        {
            Bullet bullet = new Bullet(
                GunPoint.position,
                (AimPoint.position - GunPoint.position).normalized * bulletSpeed,
                Instantiate(tracerEffect)
            );

            bullets.AddLast(bullet);
        }

        public void UpdateBullets(CharacterControl shooter, float deltaTime)
        {
            LinkedListNode<Bullet> currentNode = bullets.First;

            while (currentNode != null)
            {
                LinkedListNode<Bullet> nextNode = currentNode.Next;
                Bullet bullet = currentNode.Value;

                Vector3 currentPosition = bullet.GetPosition();
                bullet.SetLifetime(bullet.Lifetime + deltaTime);
                Vector3 nextPosition    = bullet.GetPosition();

                if (bullet.ShouldBeDestroyed(weaponRange))
                {
                    bullet.DestroyTracerEffect();
                    bullets.Remove(currentNode);
                    currentNode = nextNode;
                    continue;
                }

                bulletCollisionParam.StartPoint     = currentPosition;
                bulletCollisionParam.EndPoint       = nextPosition;
                bulletCollisionParam.PlayerCam      = shooter.MainCam;
                bulletCollisionParam.ShootableLayer = ShootableLayer;
                bulletCollisionParam.HitEffect      = hitEffect;
                bulletCollisionParam.HitEffectCount = hitEffectCount;

                bullet.CheckCollision(bulletCollisionParam);
                currentNode = nextNode;
            }
        }
    }
}
