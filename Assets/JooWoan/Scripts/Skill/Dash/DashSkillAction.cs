using System;
using System.Collections;
using DG.Tweening;
using EverScord.Character;
using UnityEngine;

namespace EverScord.Skill
{
    public class DashSkillAction : MonoBehaviour, ISkillAction
    {
        private CharacterControl activator;
        private DashSkill skill;
        private CooldownTimer cooldownTimer;
        private EJob ejob;
        private int skillIndex;

        private Coroutine skillCoroutine;
        public bool IsUsingSkill
        {
            get { return skillCoroutine != null; }
        }

        public void Init(CharacterControl activator, CharacterSkill skill, EJob ejob, int skillIndex)
        {
            this.activator = activator;
            this.skill = (DashSkill)skill;
            this.skillIndex = skillIndex;
            this.ejob = ejob;

            cooldownTimer = new CooldownTimer(skill.Cooldown);
            StartCoroutine(cooldownTimer.RunTimer());
        }

        public void Activate()
        {
            if (cooldownTimer.IsCooldown || IsUsingSkill)
                return;

            skillCoroutine = StartCoroutine(ActivateSkill());
        }

        private IEnumerator ActivateSkill()
        {
            cooldownTimer.ResetElapsedTime();

            float originalSpeed = activator.CharacterSpeed;
            float moveSpeed     = originalSpeed * 1.5f;
            float animatorSpeed = 1.5f;

            float decreasePercentage = 0.8f;
            float decreaseStartTime  = skill.Duration * decreasePercentage;
            float decreaseDuration   = (1 - decreasePercentage) * skill.Duration;

            activator.SetSpeed(moveSpeed);
            activator.AnimationControl.SetAnimatorSpeed(animatorSpeed);

            for (float i = 0f; i < skill.Duration; i += Time.deltaTime)
            {
                if (i >= decreaseStartTime)
                {
                    moveSpeed = Mathf.Lerp(moveSpeed, originalSpeed, decreaseDuration);
                    animatorSpeed = Mathf.Lerp(animatorSpeed, 1f, decreaseDuration);

                    activator.AnimationControl.SetAnimatorSpeed(animatorSpeed);
                    activator.SetSpeed(moveSpeed);
                }
                yield return null;
            }

            activator.SetSpeed(originalSpeed);
            activator.AnimationControl.SetAnimatorSpeed();

            skillCoroutine = null;
        }
    }
}
