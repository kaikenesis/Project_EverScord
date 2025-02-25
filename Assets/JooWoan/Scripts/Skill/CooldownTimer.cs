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
        public float ElapsedTime => elapsedTime;
        public float CooldownProgress
        {
            get { return Mathf.Clamp01(elapsedTime / Mathf.Max(0.01f, cooldown)); }
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
