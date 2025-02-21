using EverScord.Effects;
using UnityEngine;

namespace EverScord.Monster
{
    public class EnemyHitControl : MonoBehaviour
    {
        private const float BIG_DAMAGE = 15f;

        void Awake()
        {
            GameManager.Instance.InitControl(this);
        }

        public void ApplyDamageToEnemy(float hp, IEnemy monster)
        {
            monster?.DecreaseHP(hp);

            BlinkEffect blinkEffect = monster.GetBlinkEffect();

            if (blinkEffect == null)
                return;
            
            if (hp >= BIG_DAMAGE)
                blinkEffect.ChangeBlinkTemporarily(GameManager.HurtBlinkInfo);

            blinkEffect.Blink();
        }
    }
}
