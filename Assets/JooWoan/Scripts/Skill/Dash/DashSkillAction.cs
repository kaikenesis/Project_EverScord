using System.Collections;
using EverScord.Character;
using UnityEngine;

namespace EverScord.Skill
{
    public class DashSkillAction : SkillAction
    {
        private const float SPEED_DROP_POINT = 0.7f;
        private const float SPEED_DROP_RATE = 0.02f;

        private DashSkill skill;
        public DashSkill Skill => skill;
        private MeshTrail meshTrail;
        private Coroutine trailCoroutine;
        private GameObject tornadoEffect;
        private float originalSpeed;

        public override bool CanAttackWhileSkill => true;
        public override bool IsUsingSkill
        {
            get
            {
                if (skillCoroutine != null)
                    return true;
                
                return trailCoroutine != null;
            }
        }

        public override void Init(CharacterControl activator, CharacterSkill skill, PlayerData.EJob ejob, int skillIndex)
        {
            base.Init(activator, skill, ejob, skillIndex);

            this.skill = (DashSkill)skill;
            meshTrail  = new MeshTrail(activator.PlayerTransform, this);
            originalSpeed = activator.Speed;
        }

        public override bool Activate()
        {
            if (!base.Activate())
                return false;

            trailCoroutine = StartCoroutine(meshTrail.ActivateTrail(skill.Duration));
            skillCoroutine = StartCoroutine(ActivateSkill());
            return true;
        }

        private IEnumerator ActivateSkill()
        {
            float animatorSpeed = skill.SpeedMultiplier;
            float moveSpeed     = originalSpeed * skill.SpeedMultiplier;

            float decreaseValue = SPEED_DROP_POINT;
            float decreaseStartTime = skill.Duration * SPEED_DROP_POINT;

            activator.SetSpeed(moveSpeed);
            activator.AnimationControl.SetAnimatorSpeed(animatorSpeed);
            activator.PhysicsControl.AddImpact(activator.LookDir, skill.DashForce);

            tornadoEffect = Instantiate(skill.DashTornado, activator.PlayerTransform);
            GameObject sparkEffect   = Instantiate(skill.DashSpark, activator.PlayerTransform);

            tornadoEffect.transform.position = activator.PlayerTransform.position;
            sparkEffect.transform.position   = activator.PlayerTransform.position;

            for (float i = 0f; i < skill.Duration; i += Time.deltaTime)
            {
                if (i >= decreaseStartTime)
                {
                    decreaseValue += SPEED_DROP_RATE;

                    animatorSpeed = Mathf.Lerp(animatorSpeed, 1f, decreaseValue);
                    moveSpeed     = Mathf.Lerp(moveSpeed, originalSpeed, decreaseValue);

                    activator.AnimationControl.SetAnimatorSpeed(animatorSpeed);
                    activator.SetSpeed(moveSpeed);
                }
                yield return null;
            }

            ExitSkill();
        }

        public override void OffensiveAction()
        {
            throw new System.NotImplementedException();
        }

        public override void SupportAction()
        {
            throw new System.NotImplementedException();
        }

        public override void ExitSkill()
        {
            activator.SetSpeed(originalSpeed);
            activator.AnimationControl.SetAnimatorSpeed();

            // Effect will be automatically destroyed due to particle system settings
            if (tornadoEffect)
                tornadoEffect.GetComponent<ParticleSystem>().Stop();

            if (trailCoroutine != null)
                StopCoroutine(trailCoroutine);

            if (skillCoroutine != null)
                StopCoroutine(skillCoroutine);

            trailCoroutine = null;
            skillCoroutine = null;
        }
    }
}
