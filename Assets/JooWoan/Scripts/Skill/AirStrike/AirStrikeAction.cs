using EverScord.Character;
using System.Collections;
using UnityEngine;

namespace EverScord.Skill
{
    public class AirStrikeAction : MonoBehaviour, ISkillAction
    {
        private CharacterControl activator;
        private AirStrikeSkill skill;
        private EJob ejob;
        private int skillIndex;

        private CooldownTimer cooldownTimer;
        private TrajectoryPredictor predictor;
        private Coroutine skillCoroutine, predictionCoroutine;
        private Transform throwPoint;

        private bool hasActivated = false;

        public bool IsUsingSkill { get { return skillCoroutine != null; } }
        public bool CanAttackWhileSkill => false;

        public void Init(CharacterControl activator, CharacterSkill skill, EJob ejob, int skillIndex)
        {
            this.activator = activator;
            this.skill = (AirStrikeSkill)skill;
            this.skillIndex = skillIndex;
            this.ejob = ejob;

            throwPoint = Instantiate(this.skill.ThrowPoint, activator.transform).transform;
            predictor = new TrajectoryPredictor(activator, transform , throwPoint);

            cooldownTimer = new CooldownTimer(skill.Cooldown);
            StartCoroutine(cooldownTimer.RunTimer());
        }

        public void Activate()
        {
            if (cooldownTimer.IsCooldown)
                return;

            hasActivated = !hasActivated;

            if (!hasActivated)
            {
                ExitSkill();
                return;
            }

            skillCoroutine = StartCoroutine(ActivateSkill());
        }

        private IEnumerator ActivateSkill()
        {
            predictionCoroutine = StartCoroutine(predictor.Activate());

            while (true)
            {
                if (activator.PlayerInputInfo.pressedLeftMouseButton && !predictor.IsTargetMoving)
                {
                    activator.SetMouseButtonDown(false);
                    StartCoroutine(predictor.ThrowTarget(skill.Flare));
                    ExitSkill();
                    yield break;
                }

                yield return null;
            }
        }

        private void ExitSkill()
        {
            hasActivated = false;
            predictor.SetPathVisibility(false);

            if (predictionCoroutine != null)
            {
                StopCoroutine(predictionCoroutine);
                predictionCoroutine = null;
            }

            if (skillCoroutine != null)
            {
                StopCoroutine(skillCoroutine);
                skillCoroutine = null;
            }
        }

        public void OffensiveAction()
        {
            throw new System.NotImplementedException();
        }

        public void SupportAction()
        {
            throw new System.NotImplementedException();
        }
    }
}