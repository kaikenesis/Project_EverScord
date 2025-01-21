using System.Collections.Generic;
using UnityEngine;

namespace EverScord.Character
{
    public class AnimationSwitch : MonoBehaviour
    {
        [SerializeField] private Animator anim;

        private void SwitchAnimation(string clipName)
        {
            anim.Play(clipName, -1, 0f);
        }

        private void SwitchToIdle()
        {
            anim.SetTrigger("idleTrigger");
        }
    }
}
