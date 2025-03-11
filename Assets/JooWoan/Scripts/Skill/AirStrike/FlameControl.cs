using System;
using System.Collections.Generic;
using UnityEngine;
using EverScord.Character;

namespace EverScord.Skill
{
    public class FlameControl : MonoBehaviour
    {
        private CharacterControl activator;
        private LayerMask targetLayer;
        private IDictionary<IEnemy, CooldownTimer> hurtTimerDict;
        private float hurtInterval;
        private float flameBaseDamage;

        public void Init(CharacterControl activator, float hurtInterval, float flameBaseDamage, LayerMask targetLayer)
        {
            this.activator = activator;
            this.hurtInterval = hurtInterval;
            this.flameBaseDamage = flameBaseDamage;
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

            // Calculate flame total damage based on character stats
            float totalFlameDamage = flameBaseDamage;

            GameManager.Instance.EnemyHitsControl.ApplyDamageToEnemy(activator, totalFlameDamage, enemy, true);
            hurtTimerDict[enemy].ResetElapsedTime();
        }
    }
}
