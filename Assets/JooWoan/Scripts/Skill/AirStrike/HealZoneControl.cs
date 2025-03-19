using System.Collections.Generic;
using UnityEngine;
using EverScord.Character;

namespace EverScord.Skill
{
    public class HealZoneControl : MonoBehaviour
    {
        private CharacterControl activator;
        private CharacterSkillInfo skillInfo;
        private LayerMask targetLayer;
        private IDictionary<CharacterControl, CooldownTimer> healTimerDict;
        private float healInterval;
        private bool isInitialized = false;

        public void Init(CharacterControl activator, float healInterval, CharacterSkillInfo skillInfo, LayerMask targetLayer)
        {
            this.activator = activator;
            this.healInterval = healInterval;
            this.skillInfo = skillInfo;
            this.targetLayer = targetLayer;

            healTimerDict = new Dictionary<CharacterControl, CooldownTimer>();
            isInitialized = true;
        }

        void OnTriggerStay(Collider other)
        {
            if (!isInitialized)
                return;

            if (((1 << other.gameObject.layer) & targetLayer) == 0)
                return;

            CharacterControl player = other.transform.GetComponent<CharacterControl>();
            
            if (!healTimerDict.ContainsKey(player))
            {
                healTimerDict[player] = new CooldownTimer(healInterval);
                StartCoroutine(healTimerDict[player].RunTimer());
            }

            if (healTimerDict[player].IsCooldown)
                return;

            float totalHealAmount = DamageCalculator.GetSkillDamage(activator, skillInfo.skillDotDamage, skillInfo.skillCoefficient);
            player.IncreaseHP(activator, totalHealAmount, true);
            healTimerDict[player].ResetElapsedTime();
        }
    }
}
