using EverScord.Effects;
using UnityEngine;

namespace EverScord.Monster
{
    public class EnemyHitControl : MonoBehaviour
    {
        void Awake()
        {
            GameManager.Instance.InitControl(this);
        }

        public void ApplyDamageToEnemy(float hp, IEnemy monster)
        {
            monster?.DecreaseHP(hp);

            BlinkEffect blinkEffect = monster.GetBlinkEffect();

            if (blinkEffect != null)
                blinkEffect.Blink();
        }
    }
}
