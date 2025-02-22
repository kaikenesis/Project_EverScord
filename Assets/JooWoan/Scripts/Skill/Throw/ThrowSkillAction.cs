using System.Collections;
using UnityEngine;
using Photon.Pun;
using EverScord.Character;

namespace EverScord.Skill
{
    public abstract class ThrowSkillAction : SkillAction
    {
        protected TrajectoryPredictor predictor;
        protected Coroutine predictionCoroutine;
        protected bool hasActivated = false;

        public TrajectoryPredictor Predictor => predictor;
        public ThrowSkill ThrowingSkill { get; private set; }
        private WaitForSeconds waitDelay = new WaitForSeconds(0.2f);

        private AnimationParam throwReadyParam, throwParam, idleParam;

        public override void Init(CharacterControl activator, CharacterSkill skill, PlayerData.EJob ejob, int skillIndex)
        {
            ThrowingSkill = (ThrowSkill)skill;
            predictor = new TrajectoryPredictor(activator, transform, ThrowingSkill);

            throwReadyParam = new AnimationParam(activator.AnimationControl.AnimInfo.ThrowReady.name, 0.1f);
            throwParam = new AnimationParam(activator.AnimationControl.AnimInfo.Throw.name, 0.1f);
            idleParam = new AnimationParam(activator.AnimationControl.AnimInfo.Idle.name, 0.1f, 1);

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
                activator.AnimationControl.CrossFade(idleParam);

                ExitSkill();
                return false;
            }

            activator.RigControl.SetAimWeight(false);
            activator.AnimationControl.CrossFade(throwReadyParam);
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
                    activator.AnimationControl.CrossFade(throwParam);

                    cooldownTimer.ResetElapsedTime();

                    TrajectoryInfo info = predictor.GetTrajectoryInfo();
                    StartCoroutine(ThrowObject(info));
                    SendRPC(info);

                    yield return waitDelay;
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

        private void SendRPC(TrajectoryInfo info)
        {
            if (!PhotonNetwork.IsConnected)
                return;
            
            photonView.RPC(
                nameof(activator.SyncThrowSkill),
                RpcTarget.Others,
                activator.MouseRayHitPos,
                info.ThrownPosition,
                info.GroundDirection,
                info.InitialVelocity,
                info.TrajectoryAngle,
                info.EstimatedTime,
                skillIndex
            );
        }

        public abstract IEnumerator ThrowObject(TrajectoryInfo info);
    }
}
