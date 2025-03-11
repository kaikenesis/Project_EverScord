using EverScord.Character;
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

        public void ApplyDamageToEnemy(CharacterControl attacker, float hp, IEnemy monster, bool isSkillDamage = true)
        {
            if (monster is NController nctrl && nctrl.isDead)
                return;

            if (monster != null && attacker.CharacterPhotonView.IsMine)
            {
                monster.DecreaseHP(hp, attacker);
                attacker.IncreaseDealtDamage(hp);
            }

            BlinkEffect blinkEffect = monster.GetBlinkEffect();

            if (blinkEffect == null)
                return;
            
            if (isSkillDamage)
                blinkEffect.ChangeBlinkTemporarily(GameManager.HurtBlinkInfo);

            blinkEffect.Blink();
        }
    }
}
