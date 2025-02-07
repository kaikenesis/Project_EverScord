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

        }
    }
}
