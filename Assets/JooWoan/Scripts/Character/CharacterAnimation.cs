using System.Collections.Generic;
using UnityEngine.Animations.Rigging;
using UnityEngine;

namespace EverScord.Character
{
    public class CharacterAnimation
    {
        private Animator anim;
        private MultiAimConstraint aim;
        private TwoBoneIKConstraint leftHandIK;
        private float smoothRotation, transitionDampTime;

        private IDictionary<string, AnimationClip> animDict = new Dictionary<string, AnimationClip>();

        public CharacterAnimation(Animator anim, MultiAimConstraint aim, TwoBoneIKConstraint leftHand, float smooth, float damp)
        {
            this.anim           = anim;
            this.aim            = aim;
            leftHandIK          = leftHand;
            smoothRotation      = smooth;
            transitionDampTime  = damp;

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
            aim.weight = result;
            leftHandIK.weight = result;
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
