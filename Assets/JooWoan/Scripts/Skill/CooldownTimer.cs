using System;
using System.Collections;
using UnityEngine;

namespace EverScord.Skill
{
    public class CooldownTimer
    {
        protected float cooldown;
        protected float elapsedTime = 0f;
        protected Action onTimerTick;
        protected bool canRunTimer = true;

        public bool IsCooldown => elapsedTime < cooldown;
        public float Cooldown => cooldown;
        public float ElapsedTime => elapsedTime;
        public float CooldownProgress
        {
            get { return Mathf.Clamp01(elapsedTime / Mathf.Max(0.01f, cooldown)); }
        }

        public CooldownTimer(float cooldown, Action callback = null)
        {
            onTimerTick -= callback;
            onTimerTick += callback;

            this.cooldown = cooldown;
            elapsedTime = cooldown;
        }

        public virtual IEnumerator RunTimer(bool resetTime = false)
        {
            canRunTimer = true;

            if (resetTime)
                ResetElapsedTime();

            while (canRunTimer)
            {
                elapsedTime += Time.deltaTime;
                onTimerTick?.Invoke();
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

        public void SetNewCooldown(float cooldown)
        {
            this.cooldown = cooldown;
        }

        public void StopTimer()
        {
            canRunTimer = false;
        }

        public void SubscribeOnTimerTick(Action subscriber)
        {
            onTimerTick -= subscriber;
            onTimerTick += subscriber;
        }

        public void UnsubscribeOnTimerTick(Action subscriber)
        {
            onTimerTick -= subscriber;
        }
    }
}
