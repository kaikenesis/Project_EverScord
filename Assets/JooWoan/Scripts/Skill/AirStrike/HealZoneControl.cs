using System.Collections.Generic;
using System;
using UnityEngine;
using EverScord.Character;

namespace EverScord.Skill
{
    public class HealZoneControl : MonoBehaviour
    {
        private LayerMask targetLayer;
        private IDictionary<CharacterControl, CooldownTimer> healTimerDict;
        private float healInterval;
        private float healBaseAmount;

        public void Init(float healInterval, float healBaseAmount, LayerMask targetLayer)
        {
            this.healInterval = healInterval;
            this.healBaseAmount = healBaseAmount;
            this.targetLayer = targetLayer;

            healTimerDict = new Dictionary<CharacterControl, CooldownTimer>();
        }

        void OnTriggerStay(Collider other)
        {
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

            // Calculate total heal based on character stats
            float totalHealAmount = healBaseAmount;

            player.IncreaseHP(totalHealAmount);            
            healTimerDict[player].ResetElapsedTime();
        }
    }
}
