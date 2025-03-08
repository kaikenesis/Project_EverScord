using System.Collections;
using UnityEngine;

namespace EverScord.Skill
{
    public class CooldownTimer
    {
        protected float cooldown;
        protected float elapsedTime = 0f;

        public bool IsCooldown => elapsedTime < cooldown;
        public float Cooldown => cooldown;
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

        public virtual IEnumerator RunTimer(bool resetTime = false)
        {
            if (resetTime)
                ResetElapsedTime();

            while (true)
            {
                elapsedTime += Time.deltaTime;
                yield return null;
            }
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
