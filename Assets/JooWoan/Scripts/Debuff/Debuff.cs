using EverScord.Character;
using UnityEngine;

namespace EverScord
{
    public abstract class Debuff : MonoBehaviour
    {
        public abstract void Release();

        public static Debuff GetDebuff(CharState state)
        {
            switch (state)
            {
                case CharState.STUNNED:
                    return new StunnedDebuff(15);

                default:
                    return null;
            }
        }
    }
}
