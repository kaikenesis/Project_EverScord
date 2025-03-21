using System;
using UnityEngine;
using EverScord.Character;

namespace EverScord.Skill
{
    public abstract class ThrowableImpact : MonoBehaviour
    {
        private const float ESTIMATED_TIME_DELAY = 0.1f;
        protected Action onSkillActivated = null;
        protected ThrowSkillAction skillAction = null;
        protected CharacterControl thrower;
        protected CooldownTimer timer = null;

        void Update()
        {
            transform.Rotate(10, 0, 0);
            if (timer == null || timer.IsCooldown)
                return;

            Impact();
            Destroy(gameObject);
        }

        protected void OnCollisionEnter(Collision collision)
        {
            if (!IsValidCollision(collision))
                return;

            Impact();
            Destroy(gameObject);
        }

        protected abstract void Impact();

        public virtual void Init(CharacterControl activator, ThrowSkillAction skillAction, TrajectoryInfo info)
        {
            this.skillAction = skillAction;
            thrower = activator;
            
            onSkillActivated = (thrower.CharacterJob == PlayerData.EJob.Dealer) ? skillAction.OffensiveAction : skillAction.SupportAction;

            timer = new CooldownTimer(info.EstimatedTime + ESTIMATED_TIME_DELAY);
            StartCoroutine(timer.RunTimer(true));
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
