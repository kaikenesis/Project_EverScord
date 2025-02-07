using System.Collections.Generic;
using UnityEngine;
using EverScord.Pool;

namespace EverScord.Weapons
{
    public class BulletControl : MonoBehaviour
    {
        private LinkedList<Bullet> bullets = new();
        public LinkedList<Bullet> BulletList => bullets;

        void Update()
        {
            UpdateBullets(Time.deltaTime);
        }

        public void AddBullet(Bullet bullet)
        {
            bullets.AddLast(bullet);
        }

        private void UpdateBullets(float deltaTime)
        {
            if (bullets.Count == 0)
                return;
            
            LinkedListNode<Bullet> currentNode = bullets.First;

            while (currentNode != null)
            {
                LinkedListNode<Bullet> nextNode = currentNode.Next;
                Bullet bullet = currentNode.Value;

                Vector3 currentPosition = bullet.GetPosition();
                bullet.SetLifetime(bullet.Lifetime + deltaTime);
                Vector3 nextPosition    = bullet.GetPosition();

                if (bullet.ShouldBeDestroyed(bullet.SourceWeapon.WeaponRange))
                {
                    PoolManager.Return(bullet, bullet.SourceWeapon.WeaponTracerType);
                    bullet.SetIsDestroyed(true);
                    bullets.Remove(currentNode);
                    currentNode = nextNode;
                    continue;
                }

                bullet.CheckCollision(currentPosition, nextPosition);
                currentNode = nextNode;
            }
        }
    }
}
