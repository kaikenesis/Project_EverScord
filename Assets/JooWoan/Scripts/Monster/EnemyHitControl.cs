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

        public void ApplyDamageToEnemy(float hp, IEnemy monster, bool isSkillDamage = true)
        {
            if (monster is NController nctrl && nctrl.isDead)
                return;

            monster?.DecreaseHP(hp);

            if (monster is BossRPC)
                GameManager.Instance.LevelController.IncreaseBossProgress((BossRPC)monster);

            BlinkEffect blinkEffect = monster.GetBlinkEffect();

            if (blinkEffect == null)
                return;
            
            if (isSkillDamage)
                blinkEffect.ChangeBlinkTemporarily(GameManager.HurtBlinkInfo);

            blinkEffect.Blink();
        }
    }
}
