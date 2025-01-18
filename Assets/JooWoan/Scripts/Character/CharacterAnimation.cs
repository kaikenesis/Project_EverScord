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
        private int adjustLayer;

        public CharacterAnimation(Animator anim, float smoothRotation, float transitionDampTime)
        {
            this.anim = anim;
            this.smoothRotation = smoothRotation;
            this.transitionDampTime = transitionDampTime;

            adjustLayer = anim.GetLayerIndex("AdjustMask");
            anim.SetLayerWeight(adjustLayer, 0f);

            foreach (AnimationClip clip in anim.runtimeAnimatorController.animationClips)
                animDict[clip.name] = clip;
        }

        public void AnimateMovement(Vector3 moveDir)
        {
            anim.SetFloat("Horizontal", moveDir.x, transitionDampTime, Time.deltaTime);
            anim.SetFloat("Vertical", moveDir.z, transitionDampTime, Time.deltaTime);
        }

        public void AdjustPosture(bool shouldAdjust)
        {
            float layerWeight = anim.GetLayerWeight(adjustLayer);

            if (shouldAdjust && layerWeight == 0f)
            {
                anim.SetLayerWeight(adjustLayer, 1f);
                anim.Play("AdjustShoot", -1, 0f);
            }
            else if (!shouldAdjust && layerWeight > 0)
            {
                anim.SetLayerWeight(adjustLayer, 0f);
                anim.Play("AdjustReset", -1, 0f);
            }
        }

        public void Play(string clipName)
        {
            anim.Play(clipName, -1, 0f);
        }
    }
}
