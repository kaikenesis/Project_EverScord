using System.Collections.Generic;
using UnityEngine;
using EverScord.Character;

namespace EverScord.Skill
{
    public class HealZoneControl : MonoBehaviour
    {
        private CharacterControl activator;
        private LayerMask targetLayer;
        private IDictionary<CharacterControl, CooldownTimer> healTimerDict;
        private float healInterval;
        private float healBaseAmount;
        private bool isInitialized = false;

        public void Init(CharacterControl activator, float healInterval, float healBaseAmount, LayerMask targetLayer)
        {
            this.activator = activator;
            this.healInterval = healInterval;
            this.healBaseAmount = healBaseAmount;
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

            // Calculate total heal based on character stats
            float totalHealAmount = healBaseAmount;

            player.IncreaseHP(activator, totalHealAmount, true);
            healTimerDict[player].ResetElapsedTime();
        }
    }
}
