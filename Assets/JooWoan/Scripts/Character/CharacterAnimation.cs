using UnityEngine;

namespace EverScord.Character
{
    public class CharacterAnimation
    {
        private Animator anim;
        private float transitionDampTime;
        public AnimationInfo AnimInfo { get; private set; }

        public CharacterAnimation(Animator anim, AnimationInfo info, float dampTime)
        {
            this.anim           = anim;
            AnimInfo            = info;
            transitionDampTime  = dampTime;
        }

        public void AnimateMovement(CharacterControl character, Vector3 moveDir)
        {
            if (!character.IsMoving)
            {
                anim.SetBool(ConstStrings.PARAM_ISMOVING, false);
                return;
            }

            anim.SetBool(ConstStrings.PARAM_ISMOVING, true);
            anim.SetFloat(ConstStrings.PARAM_HORIZONTAL, moveDir.x, transitionDampTime, Time.deltaTime);
            anim.SetFloat(ConstStrings.PARAM_VERTICAL, moveDir.z, transitionDampTime, Time.deltaTime);
        }

        public void Rotate(bool state)
        {
            anim.SetBool(ConstStrings.PARAM_ISROTATING, state);
        }

        public void SetBool(string name, bool state)
        {
            anim.SetBool(name, state);
        }

        public void Play(AnimationClip clip)
        {
            anim.Play(clip.name, -1, 0f);
        }
    }
}
