using System.Collections.Generic;
using UnityEngine;

namespace EverScord.Character
{
    public class AnimationSwitch : MonoBehaviour
    {
        [SerializeField] private Animator anim;
        private int upperLayer;

        void Start()
        {
            upperLayer = anim.GetLayerIndex("UpperBody");
        }

        private void SwitchAnimation(string clipName)
        {
            anim.Play(clipName, upperLayer, 0f);
        }

        private void SwitchToIdle()
        {
            anim.SetTrigger("idleTrigger");
        }
    }
}
