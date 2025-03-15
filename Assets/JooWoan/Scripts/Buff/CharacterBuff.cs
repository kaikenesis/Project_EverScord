using System;
using EverScord.Skill;

namespace EverScord.Character
{
    public abstract class CharacterBuff
    {
        protected CooldownTimer buffTimer;
        protected Action onBuffEnd;
        protected CharacterControl target;

        private bool isBuffOver = false;
        public bool IsBuffOver => isBuffOver;

        public static CharacterBuff GetBuff(CharacterControl target, BuffType type, float duration, Action onBuffEnd)
        {
            CharacterBuff targetBuff = null;

            switch (type)
            {
                case BuffType.BARRIER:
                    targetBuff = new BarrierBuff();
                    break;
            }

            return targetBuff?.Init(target, duration, onBuffEnd);
        }

        private CharacterBuff Init(CharacterControl target, float duration, Action onBuffEnd)
        {
            this.target = target;
            this.onBuffEnd = onBuffEnd;

            buffTimer = new CooldownTimer(duration, CheckBuffEnd);
            target.StartCoroutine(buffTimer.RunTimer(true));

            Apply();

            return this;
        }

        protected abstract void Apply();

        private void CheckBuffEnd()
        {
            if (!buffTimer.IsCooldown && !isBuffOver)
            {
                isBuffOver = true;
                buffTimer.StopTimer();
                EndBuff();
            }
        }

        protected virtual void EndBuff()
        {
            onBuffEnd?.Invoke();
        }
    }

    public enum BuffType
    {
        BARRIER,
    }
}
