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
            activator.SetSpeed(originalSpeed * 1.5f);

            yield return new WaitForSeconds(skill.Duration);

            activator.SetSpeed(originalSpeed);
            skillCoroutine = null;
        }
    }
}
