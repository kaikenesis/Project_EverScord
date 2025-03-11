using System;

namespace EverScord.Skill
{
    public class SkillTimer : CooldownTimer
    {
        // int: skill index, float: cooldown duration
        private static Action<int, float> onCoolDown;
        private int skillIndex;
        private bool isMine;

        public SkillTimer(float cooldown, int skillIndex, bool isMine) : base(cooldown)
        {
            this.skillIndex = skillIndex;
            this.isMine = isMine;

            onTimerTick -= UpdateCooldownProgress;
            onTimerTick += UpdateCooldownProgress;
        }

        public static void SubscribeOnCooldown(Action<int, float> subscriber)
        {
            onCoolDown -= subscriber;
            onCoolDown += subscriber;
        }

        public static void UnsubscribeOnCooldown(Action<int, float> subscriber)
        {
            onCoolDown -= subscriber;
        }

        private void UpdateCooldownProgress()
        {
            if (IsCooldown && isMine)
                onCoolDown?.Invoke(skillIndex, CooldownProgress);
        }
    }
}