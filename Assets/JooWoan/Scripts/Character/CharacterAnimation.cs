using System.Collections.Generic;
using UnityEngine;

namespace EverScord.Character
{
    public class CharacterAnimation
    {
        public Animator anim { get; private set; }
        public float smoothRotation { get; private set; }
        public float transitionDampTime { get; private set; }

        private IDictionary<string, AnimationClip> animDict = new Dictionary<string, AnimationClip>();

        public CharacterAnimation(Animator anim, float smoothRotation, float transitionDampTime)
        {
            this.anim = anim;
            this.smoothRotation = smoothRotation;
            this.transitionDampTime = transitionDampTime;

            foreach (AnimationClip clip in anim.runtimeAnimatorController.animationClips)
                animDict[clip.name] = clip;
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

        public void SetAimRig(CharacterControl character)
        {
            float result = character.IsAiming ? 1f : 0f;
            character.Aim.weight = result;
            character.LeftHandIK.weight = result;
        }

        public void Rotate(bool state)
        {
            anim.SetBool(ConstStrings.PARAM_ISROTATING, state);
        }

        public void Play(string clipName)
        {
            anim.Play(clipName, -1, 0f);
        }
    }
}
