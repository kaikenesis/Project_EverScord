using System;
using EverScord.Character;

namespace EverScord
{
    public abstract class Debuff
    {
        protected Action<CharState> onDebuffRemoved;
        protected CharState linkedState;
        protected CharacterControl target;

        public static Debuff GetDebuff(CharacterControl target, CharState state, Action<CharState> onDebuffRemoved)
        {
            Debuff targetDebuff = null;

            switch (state)
            {
                case CharState.STUNNED:
                    targetDebuff = new StunnedDebuff();
                    break;
            }

            return targetDebuff?.Init(target, state, onDebuffRemoved).ShowDebuffEffect();
        }

        protected Debuff Init(CharacterControl target, CharState state, Action<CharState> onDebuffRemoved)
        {
            this.target = target;

            this.onDebuffRemoved -= onDebuffRemoved;
            this.onDebuffRemoved += onDebuffRemoved;

            linkedState = state;
            return this;
        }

        protected virtual Debuff ShowDebuffEffect()
        {
            return this;
        }

        public virtual void RemoveDebuff()
        {
            onDebuffRemoved?.Invoke(linkedState);
            onDebuffRemoved = null;
        }
    }
}
