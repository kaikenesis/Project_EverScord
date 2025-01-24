using System.Collections;
using UnityEngine;
using EverScord.Character;
using System.Collections.Generic;

namespace EverScord.Weapons
{
    public delegate void OnShotFired(int count);

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
        [field: SerializeField] public float ReloadTime             { get; private set; }
        [field: SerializeField] public int MaxAmmo                  { get; private set; }

        [SerializeField] private float weaponRange;
        [SerializeField] public float bulletSpeed;
        [SerializeField] private int hitEffectCount;

        private OnShotFired onShotFired;

        private LinkedList<Bullet> bullets = new();
        private BulletCollisionParam bulletCollisionParam = new();
        private float elapsedTime;
        private int currentAmmo;
        private bool isCooldown => elapsedTime < Cooldown;
        private bool isReloading = false;

        public void Init(OnShotFired setText)
        {
            AimPoint = Instantiate(AimPoint).transform;
            currentAmmo = MaxAmmo;

            onShotFired -= setText;
            onShotFired += setText;
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
            if (isReloading)
                return;

            float cooldownOvertime = elapsedTime - Cooldown;

            if (shooter.IsAiming && (cooldownOvertime > shooter.ShootStanceDuration))
            {
                shooter.SetIsAiming(false);
                shooter.AnimationControl.SetAimRig(shooter);
                shooter.AnimationControl.Play(ConstStrings.ANIMATION_NED_SHOOT_END);
                return;
            }

            if (!CanShoot(shooter))
                return;

            if (currentAmmo <= 0)
            {
                StartCoroutine(ReloadWeapon(shooter));
                return;
            }

            --CurrentAmmo;
            elapsedTime = 0f;

            shotEffect.Emit(1);

            shooter.SetIsAiming(true);
            shooter.AnimationControl.SetAimRig(shooter);
            shooter.AnimationControl.Play(ConstStrings.ANIMATION_NED_SHOOT);

            FireBullet();
        }

        private IEnumerator ReloadWeapon(CharacterControl shooter)
        {
            isReloading = true;

            shooter.AnimationControl.SetAimRig(false);
            shooter.AnimationControl.Play(ConstStrings.ANIMATION_NED_RELOAD);
            
            StartCoroutine(shooter.PlayerUIControl.RollAmmoText(this));

            yield return new WaitForSeconds(ReloadTime);

            CurrentAmmo = MaxAmmo;
            elapsedTime = 0f;

            shooter.SetIsAiming(true);
            shooter.AnimationControl.SetAimRig(true);
            shooter.AnimationControl.Trigger(ConstStrings.PARAM_STANCE_TRIGGER);

            isReloading = false;
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
                bulletCollisionParam.PlayerCam      = shooter.CameraControl.Cam;
                bulletCollisionParam.ShootableLayer = ShootableLayer;
                bulletCollisionParam.HitEffect      = hitEffect;
                bulletCollisionParam.HitEffectCount = hitEffectCount;

                bullet.CheckCollision(bulletCollisionParam);
                currentNode = nextNode;
            }
        }

        private bool CanShoot(CharacterControl shooter)
        {
            if (isCooldown)
                return false;

            if (!shooter.IsShooting)
                return false;

            return true;
        }

        public int CurrentAmmo
        {
            get { return currentAmmo; }
            set
            {
                currentAmmo = value;

                if (onShotFired != null)
                    onShotFired(currentAmmo);
            }
        }
    }
}
