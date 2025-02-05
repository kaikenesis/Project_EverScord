using System.Collections;
using UnityEngine;
using EverScord.Character;
using System.Collections.Generic;
using EverScord.Pool;
using EverScord.Skill;

namespace EverScord.Weapons
{
    public delegate void OnShotFired(int count);

    public class Weapon : MonoBehaviour
    {
        [SerializeField] private ParticleSystem shotEffect, hitEffect;
        [SerializeField] private TracerType tracerType;
        [field: SerializeField] public Transform GunPoint           { get; private set; }
        [field: SerializeField] public Transform AimPoint           { get; private set; }
        [field: SerializeField] public Transform LeftTarget         { get; private set; }
        [field: SerializeField] public Transform LeftHint           { get; private set; }
        [field: SerializeField] public LayerMask ShootableLayer     { get; private set; }
        [field: SerializeField] public float AimSensitivity         { get; private set; }
        [field: SerializeField] public float MinAimDistance         { get; private set; }
        [field: SerializeField] public float Cooldown               { get; private set; }
        [field: SerializeField] public float ReloadTime             { get; private set; }
        [field: SerializeField] public int MaxAmmo                  { get; private set; }

        [SerializeField] private float weaponRange;
        [SerializeField] public float bulletSpeed;
        [SerializeField] private int hitEffectCount;

        [SerializeField] private GameObject smokePrefab;

        private OnShotFired onShotFired;
        private LinkedList<Bullet> bullets = new();
        private BulletCollisionParam bulletCollisionParam = new();
        private CooldownTimer cooldownTimer;

        private const float ANIM_TRANSITION = 0.25f;
        private int currentAmmo;

        private bool isReloading = false;
        public bool IsReloading => isReloading;

        public void Init(OnShotFired setText)
        {
            AimPoint = Instantiate(AimPoint).transform;
            currentAmmo = MaxAmmo;

            onShotFired -= setText;
            onShotFired += setText;
        }

        void OnEnable()
        {
            if (cooldownTimer == null)
                cooldownTimer = new CooldownTimer(Cooldown);

            StartCoroutine(cooldownTimer.RunTimer());
        }

        void OnDisable()
        {
            cooldownTimer.StopTimer();
        }

        public void Shoot(CharacterControl shooter)
        {
            if (isReloading)
                return;

            if (shooter.IsUsingSkill)
                return;

            float cooldownOvertime = cooldownTimer.GetElapsedTime() - Cooldown;
            CharacterAnimation animControl = shooter.AnimationControl;

            if (shooter.IsAiming && (cooldownOvertime > shooter.ShootStanceDuration))
            {
                shooter.SetIsAiming(false);
                shooter.RigControl.SetAimWeight(false);
                animControl.Play(animControl.AnimInfo.ShootEnd);
                return;
            }

            if (!CanShoot(shooter))
                return;

            if (currentAmmo <= 0)
            {
                StartCoroutine(Reload(shooter));
                return;
            }

            --CurrentAmmo;
            cooldownTimer.ResetElapsedTime();
            shotEffect.Emit(1);

            shooter.SetIsAiming(true);
            shooter.RigControl.SetAimWeight(true);
            animControl.Play(animControl.AnimInfo.Shoot);

            FireBullet();
        }

        public void TryReload(CharacterControl shooter)
        {
            if (currentAmmo == MaxAmmo)
                return;

            if (isReloading)
                return;

            if (shooter.IsUsingSkill)
                return;

            if (!shooter.PlayerInputInfo.pressedReloadButton)
                return;

            StartCoroutine(Reload(shooter));
        }

        private IEnumerator Reload(CharacterControl shooter)
        {
            isReloading = true;
            CharacterAnimation animControl = shooter.AnimationControl;

            shooter.RigControl.SetAimWeight(false);
            shooter.PlayerUIControl.SetReloadText();

            animControl.SetBool(ConstStrings.PARAM_ISRELOADING, true);
            animControl.Play(animControl.AnimInfo.Reload);

            yield return new WaitForSeconds(ReloadTime);

            shooter.SetIsAiming(true);
            shooter.RigControl.SetAimWeight(true);
            animControl.SetBool(ConstStrings.PARAM_ISRELOADING, false);

            yield return new WaitForSeconds(animControl.AnimInfo.ShootStance.length * ANIM_TRANSITION);

            CurrentAmmo = MaxAmmo;
            cooldownTimer.ResetElapsedTime();
            isReloading = false;
        }

        private void FireBullet()
        {
            Bullet bullet = PoolManager.Get(tracerType);
            Vector3 bulletDir = (AimPoint.position - GunPoint.position).normalized;

            bullet.Init(
                GunPoint.position,
                bulletDir * bulletSpeed
            );

            bullets.AddLast(bullet);

            SmokeTrail smokeTrail = PoolManager.GetSmoke();
            smokeTrail.transform.forward = bulletDir;
            smokeTrail.Init(bullet);
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
                    PoolManager.Return(bullet, tracerType);
                    bullet.SetIsDestroyed(true);
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
            if (cooldownTimer.IsCooldown)
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
