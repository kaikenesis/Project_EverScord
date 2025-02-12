using UnityEngine;

namespace EverScord.Monster
{
    public class EnemyHitControl : MonoBehaviour
    {
        void Awake()
        {
            GameManager.Instance.InitControl(this);
        }

        public void ApplyDamageToEnemy(int damage, NController monster)
        {
            // monster.ApplyDamage(damage);
        }
    }
}
