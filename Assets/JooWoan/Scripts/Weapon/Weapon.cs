using UnityEngine;
using EverScord.Character;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;

namespace EverScord.Weapons
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] private ParticleSystem shotEffect, hitEffect;
        [SerializeField] private TrailRenderer tracerEffect;
        [field: SerializeField] public Transform GunPoint           { get; private set; }
        [field: SerializeField] public Transform AimPoint           { get; private set; }
        [field: SerializeField] public float AimSensitivity         { get; private set; }
        [field: SerializeField] public float MinAimDistance         { get; private set; }
        [field: SerializeField] public float Cooldown               { get; private set; }

        [SerializeField] private float weaponRange;
        [SerializeField] public float bulletSpeed;
        [SerializeField] private int hitEffectCount;

        private Ray ray;
        private RaycastHit rayHit;
        private LinkedList<Bullet> bullets = new();
        private float elapsedTime;
        private bool isCooldown => elapsedTime < Cooldown;

        public void CreateAimPoint()
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

        public void UpdateBullets(float deltaTime)
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
                    Destroy(bullet.TracerEffect.gameObject);
                    bullets.Remove(currentNode);
                    currentNode = nextNode;
                    continue;
                }

                RaycastSegment(currentPosition, nextPosition, bullet);
                currentNode = nextNode;
            }
        }

        private void RaycastSegment(Vector3 startPoint, Vector3 endPoint, Bullet bullet)
        {
            Vector3 direction = endPoint - startPoint;
            float distance = direction.magnitude;

            ray.origin = startPoint;
            ray.direction = direction;

            if (Physics.Raycast(ray, out rayHit, distance))
            {
                hitEffect.transform.position = rayHit.point;
                hitEffect.transform.forward  = rayHit.normal;
                hitEffect.Emit(hitEffectCount);

                bullet.SetTracerEffectPosition(rayHit.point);
                bullet.SetIsDestroyed(true);
                return;
            }

            bullet.SetTracerEffectPosition(endPoint); 
        }
    }
}
