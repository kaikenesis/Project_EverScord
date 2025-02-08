using System.Collections.Generic;
using UnityEngine;

namespace EverScord.Weapons
{
    public class BulletControl : MonoBehaviour
    {
        private LinkedList<Bullet> myBullets, otherBullets;

        void Awake()
        {
            myBullets = new();
            otherBullets = new();
        }

        void Update()
        {
            UpdateBullets(Time.deltaTime, BulletOwner.MINE);
            UpdateBullets(Time.deltaTime, BulletOwner.OTHER);
        }

        public void AddBullet(Bullet bullet, BulletOwner type)
        {
            switch (type)
            {
                case BulletOwner.MINE:
                    myBullets.AddLast(bullet);
                    break;

                case BulletOwner.OTHER:
                    otherBullets.AddLast(bullet);
                    break;

                default:
                    break;
            }
        }

        public void UpdateBullets(float deltaTime, BulletOwner type)
        {
            LinkedList<Bullet> bullets = myBullets;

            if (type == BulletOwner.OTHER)
                bullets = otherBullets;

            if (bullets.Count == 0)
                return;

            LinkedListNode<Bullet> currentNode = bullets.First;

            while (currentNode != null)
            {
                LinkedListNode<Bullet> nextNode = currentNode.Next;

                Bullet bullet = currentNode.Value;
                Weapon weapon = GameManager.Instance.PlayerDict[bullet.ViewID].PlayerWeapon;

                Vector3 currentPosition = bullet.GetPosition();
                bullet.SetLifetime(bullet.Lifetime + deltaTime);
                Vector3 nextPosition    = bullet.GetPosition();

                if (bullet.ShouldBeDestroyed(weapon.WeaponRange))
                {
                    bullet.SetIsDestroyed(true);
                    bullets.Remove(currentNode);

                    ResourceManager.instance.ReturnToPool(bullet.gameObject, weapon.BulletAssetReference.AssetGUID);

                    currentNode = nextNode;
                    continue;
                }

                if (type == BulletOwner.MINE)
                    bullet.CheckCollision(weapon, currentPosition, nextPosition);
                else
                    bullet.SetTracerEffectPosition(nextPosition);

                currentNode = nextNode;
            }
        }
    }

    public enum BulletOwner
    {
        MINE,
        OTHER
    }
}
