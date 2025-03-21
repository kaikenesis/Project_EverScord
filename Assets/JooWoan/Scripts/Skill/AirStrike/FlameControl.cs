using System.Collections.Generic;
using UnityEngine;
using EverScord.Character;

namespace EverScord.Skill
{
    public class FlameControl : MonoBehaviour
    {
        private CharacterControl activator;
        private CharacterSkillInfo skillInfo;
        private LayerMask targetLayer;
        private IDictionary<IEnemy, CooldownTimer> hurtTimerDict;
        private float hurtInterval;

        public void Init(CharacterControl activator, float hurtInterval, CharacterSkillInfo skillInfo, LayerMask targetLayer)
        {
            this.activator = activator;
            this.skillInfo = skillInfo;
            this.hurtInterval = hurtInterval;
            this.targetLayer = targetLayer;

            hurtTimerDict = new Dictionary<IEnemy, CooldownTimer>();
        }

        void OnTriggerStay(Collider other)
        {
            if (activator == null)
                return;
            
            if (((1 << other.gameObject.layer) & targetLayer) == 0)
                return;

            IEnemy enemy = other.transform.GetComponent<IEnemy>();
            
            if (!hurtTimerDict.ContainsKey(enemy))
            {
                hurtTimerDict[enemy] = new CooldownTimer(hurtInterval);
                StartCoroutine(hurtTimerDict[enemy].RunTimer());
            }

            if (hurtTimerDict[enemy].IsCooldown)
                return;

            float flameDamage = DamageCalculator.GetSkillDamage(activator, skillInfo.skillDotDamage, skillInfo.skillCoefficient, enemy);
            GameManager.Instance.EnemyHitsControl.ApplyDamageToEnemy(activator, flameDamage, enemy, true);
            hurtTimerDict[enemy].ResetElapsedTime();
        }
    }
}
