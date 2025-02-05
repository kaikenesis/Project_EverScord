using System.Collections;
using UnityEngine;

namespace EverScord.Skill
{
    public class CooldownTimer
    {
        private float cooldown;
        private float elapsedTime = 0f;
        public bool IsCooldown => elapsedTime < cooldown;
        private bool shouldStopTimer = false;

        public CooldownTimer(float cooldown)
        {
            this.cooldown = cooldown;
            elapsedTime = cooldown;
        }

        public IEnumerator RunTimer()
        {
            while (!shouldStopTimer)
            {
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }

        public void StopTimer()
        {
            shouldStopTimer = true;
        }

        public void ResetElapsedTime()
        {
            elapsedTime = 0f;
        }

        public float GetElapsedTime()
        {
            return elapsedTime;
        }
    }
}
