using UnityEngine;

namespace EverScord.Character
{
    public class CharacterAnimation
    {
        private Animator anim;
        private float smoothRotation;
        private float transitionDampTime;

        public CharacterAnimation(Animator anim, float smoothRotation, float transitionDampTime)
        {
            this.anim = anim;
            this.smoothRotation = smoothRotation;
            this.transitionDampTime = transitionDampTime;
        }

        public void AnimateMovement(Vector3 moveDir)
        {
            anim.SetFloat("Horizontal", moveDir.x, transitionDampTime, Time.deltaTime);
            anim.SetFloat("Vertical", moveDir.z, transitionDampTime, Time.deltaTime);
        }
    }

}
