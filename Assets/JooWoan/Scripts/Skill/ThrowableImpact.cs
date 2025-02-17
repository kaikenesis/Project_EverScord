using System;
using UnityEngine;
using EverScord.Character;

namespace EverScord.Skill
{
    public abstract class ThrowableImpact : MonoBehaviour
    {
        protected Action onSkillActivated = null;
        protected ThrowSkillAction skillAction = null;
        protected CharacterControl thrower;

        public abstract void OnCollisionEnter(Collision collision);

        public virtual void Init(CharacterControl activator, ThrowSkillAction skillAction)
        {
            this.skillAction = skillAction;
            thrower = activator;
            
            onSkillActivated = (thrower.CharacterJob == EJob.DEALER) ? skillAction.OffensiveAction : skillAction.SupportAction;
        }

        public bool IsValidCollision(Collision collision)
        {
            if (((1 << collision.gameObject.layer) & skillAction.ThrowingSkill.CollisionLayer) == 0)
                return false;

            if (collision.transform.root == thrower.transform)
                return false;

            return true;
        }
    }
}
