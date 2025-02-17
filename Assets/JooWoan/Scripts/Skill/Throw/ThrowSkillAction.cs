using System.Collections;
using UnityEngine;
using Photon.Pun;
using EverScord.Character;

namespace EverScord.Skill
{
    public abstract class ThrowSkillAction : SkillAction
    {
        protected TrajectoryPredictor predictor;
        public TrajectoryPredictor Predictor => predictor;
        protected Coroutine predictionCoroutine;
        protected bool hasActivated = false;

        private WaitForSeconds waitSkillEnd = new WaitForSeconds(0.1f);

        public override void Init(CharacterControl activator, CharacterSkill skill, EJob ejob, int skillIndex)
        {
            ThrowSkill throwSkill = (ThrowSkill)skill;
            predictor = new TrajectoryPredictor(activator, transform, throwSkill);

            base.Init(activator, skill, ejob, skillIndex);
        }

        public override bool Activate()
        {
            if (!activator.CharacterPhotonView.IsMine)
                return false;
            
            if (cooldownTimer.IsCooldown)
                return false;

            if (predictor.IsThrownObjectMoving)
                return false;

            hasActivated = !hasActivated;

            if (!hasActivated)
            {
                ExitSkill();
                return false;
            }

            skillCoroutine = StartCoroutine(ActivateSkill());
            return true;
        }

        protected IEnumerator ActivateSkill()
        {
            predictionCoroutine = StartCoroutine(predictor.Activate());

            while (true)
            {
                if (activator.PlayerInputInfo.pressedLeftMouseButton)
                {
                    activator.SetMouseButtonDown(false);
                    StartCoroutine(ThrowObject());
                    SendRPC();

                    yield return waitSkillEnd;
                    ExitSkill();

                    yield break;
                }

                yield return null;
            }
        }

        protected void ExitSkill()
        {
            hasActivated = false;
            predictor.Exit();

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

        private void SendRPC()
        {
            if (!PhotonNetwork.IsConnected)
                return;
            
            photonView.RPC(
                nameof(activator.SyncThrowSkill),
                RpcTarget.Others,
                activator.MouseRayHitPos,
                predictor.ThrownPosition,
                predictor.GroundDirection,
                predictor.InitialVelocity,
                predictor.TrajectoryAngle,
                predictor.EstimatedTime,
                skillIndex
            );
        }

        public abstract IEnumerator ThrowObject();
    }
}
