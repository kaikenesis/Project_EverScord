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

        public void ApplyDamageToEnemy(float hp, IEnemy monster, bool isSkillDamage = true)
        {
            if (monster is NController nctrl && nctrl.isDead)
                return;

            monster?.DecreaseHP(hp);

            BlinkEffect blinkEffect = monster.GetBlinkEffect();

            if (blinkEffect == null)
                return;
            
            if (isSkillDamage)
                blinkEffect.ChangeBlinkTemporarily(GameManager.HurtBlinkInfo);

            blinkEffect.Blink();
        }
    }
}
