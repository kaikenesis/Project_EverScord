using System.Collections;
using System;
using UnityEngine;

namespace EverScord.Skill
{
    public class SkillTimer : CooldownTimer
    {
        public static Action<int, float> OnSkillCooldown = delegate { };

        public SkillTimer(float cooldown) : base(cooldown) { }

        private void UpdateCooldownProgress()
        {
            //if (!photonView.IsMine || !cooldownTimer.IsCooldown)
            //    return;

            //OnUsedSkill?.Invoke(skillIndex, cooldownTimer.CooldownProgress);
            Debug.Log("on going");
        }

        public override IEnumerator RunTimer(bool resetTime = false)
        {
            UpdateCooldownProgress();
            yield return base.RunTimer(resetTime);
        }
    }
}

