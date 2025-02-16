using UnityEngine;
using System;
using EverScord.Character;

namespace EverScord.Skill
{
    public class GrenadeImpact : MonoBehaviour
    {
        private Action skillAction = null;
        private GrenadeSkillAction skill;

        void OnDisable()
        {
            if (skillAction == null)
                return;
            
            skill.SetGrenadeImpactPosition(transform.position);
            skillAction.Invoke();
        }

        public void Init(CharacterControl activator, GrenadeSkillAction skill)
        {
            if (!activator.CharacterPhotonView.IsMine)
                return;

            this.skill = skill;
            
            if (activator.CharacterJob == EJob.DEALER)
                skillAction = skill.OffensiveAction;
            else
                skillAction = skill.SupportAction;
        }
    }
}