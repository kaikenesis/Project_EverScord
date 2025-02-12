using System.Collections;
using EverScord.Character;
using UnityEngine;

namespace EverScord.Skill
{
    public class DashSkillAction : MonoBehaviour, ISkillAction
    {
        private const float SPEED_DROP_POINT = 0.7f;

        private CharacterControl activator;
        private DashSkill skill;
        private CooldownTimer cooldownTimer;
        private MeshTrail meshTrail;
        private Coroutine skillCoroutine, trailCoroutine;
        private EJob ejob;
        private int skillIndex;

        public DashSkill Skill => skill;
        public bool CanAttackWhileSkill => true;
        public bool IsUsingSkill
        {
            get
            {
                if (skillCoroutine != null)
                    return true;
                
                return trailCoroutine != null;
            }
        }

        public void Init(CharacterControl activator, CharacterSkill skill, EJob ejob, int skillIndex)
        {
            this.activator  = activator;
            this.skill      = (DashSkill)skill;
            this.skillIndex = skillIndex;
            this.ejob       = ejob;

            meshTrail = new MeshTrail(activator.transform, this);
        
            cooldownTimer = new CooldownTimer(skill.Cooldown);
            StartCoroutine(cooldownTimer.RunTimer());
        }

        public void Activate()
        {
            if (cooldownTimer.IsCooldown || IsUsingSkill)
                return;

            skillCoroutine = StartCoroutine(ActivateSkill());
            trailCoroutine = StartCoroutine(meshTrail.ActivateTrail(skill.Duration));
        }

        private IEnumerator ActivateSkill()
        {
            cooldownTimer.ResetElapsedTime();

            float animatorSpeed = skill.SpeedMultiplier;
            float originalSpeed = activator.CharacterSpeed;
            float moveSpeed     = originalSpeed * skill.SpeedMultiplier;

            float decreaseStartTime = skill.Duration * SPEED_DROP_POINT;
            float decreaseDuration  = (1 - SPEED_DROP_POINT) * skill.Duration;

            activator.SetSpeed(moveSpeed);
            activator.AnimationControl.SetAnimatorSpeed(animatorSpeed);
            activator.PhysicsControl.AddImpact(activator.LookDir, skill.DashForce);

            GameObject effect = Instantiate(skill.EffectPrefab, activator.transform);
            effect.transform.position = activator.transform.position;

            for (float i = 0f; i < skill.Duration; i += Time.deltaTime)
            {
                if (i >= decreaseStartTime)
                {
                    moveSpeed = Mathf.Lerp(moveSpeed, originalSpeed, decreaseDuration);
                    animatorSpeed = Mathf.Lerp(animatorSpeed, 1f, decreaseDuration);

                    activator.AnimationControl.SetAnimatorSpeed(animatorSpeed);
                    activator.SetSpeed(moveSpeed);
                }
                yield return null;
            }

            activator.SetSpeed(originalSpeed);
            activator.AnimationControl.SetAnimatorSpeed();

            // effect will be automatically destroyed due to particle system settings
            effect.GetComponent<ParticleSystem>().Stop();

            StopCoroutine(trailCoroutine);
            trailCoroutine = null;
            skillCoroutine = null;
        }
    }
}
