using System.Collections;
using UnityEngine;

namespace EverScord.Skill
{
    public class CooldownTimer
    {
        private float cooldown;
        private float elapsedTime = 0f;
        private bool shouldStopTimer = false;

        public bool IsCooldown => elapsedTime < cooldown;
        public float CooldownProgress
        {
            get
            {
                float progress = elapsedTime / Mathf.Max(cooldown, 0.01f);
                return Mathf.Clamp(progress, 0f, 100f);
            }
        }

        public CooldownTimer(float cooldown)
        {
            this.cooldown = cooldown;
            elapsedTime = cooldown;
        }

        public IEnumerator RunTimer(bool resetTime = false)
        {
            if (resetTime)
                ResetElapsedTime();

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
