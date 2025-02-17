using System;
using System.Collections.Generic;
using UnityEngine;

namespace EverScord.Skill
{
    public class FlameControl : MonoBehaviour
    {
        private Action<float, IEnemy> onEnemyHurt;
        private LayerMask targetLayer;
        private IDictionary<IEnemy, CooldownTimer> hurtTimerDict;
        private float hurtInterval;
        private float flameBaseDamage;

        public void Init(Action<float, IEnemy> onEnemyHurt, float hurtInterval, float flameBaseDamage, LayerMask targetLayer)
        {
            this.onEnemyHurt = onEnemyHurt;
            this.hurtInterval = hurtInterval;
            this.flameBaseDamage = flameBaseDamage;
            this.targetLayer = targetLayer;

            hurtTimerDict = new Dictionary<IEnemy, CooldownTimer>();
        }

        void OnTriggerStay(Collider other)
        {
            if (onEnemyHurt == null)
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
            
            onEnemyHurt.Invoke(totalFlameDamage, enemy);
            hurtTimerDict[enemy].ResetElapsedTime();
        }
    }
}
