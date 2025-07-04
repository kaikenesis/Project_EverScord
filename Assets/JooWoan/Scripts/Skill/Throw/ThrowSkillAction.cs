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
                SoundManager.Instance.PlaySound(ConstStrings.SFX_THROW_CANCEL);
                activator.AnimationControl.CrossFade(idleParam);
                ExitSkill();
                return false;
            }

            PlayMarkerSound();
            SoundManager.Instance.PlaySound(ConstStrings.SFX_READY2THROW);

            activator.RigControl.SetAimWeight(false);
            activator.AnimationControl.SetUpperMask(true);
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
                    base.Activate();
                    PlayThrowSound();

                    activator.SetMouseButtonDown(false);
                    activator.AnimationControl.CrossFade(throwParam);

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

        public override void ExitSkill()
        {
            hasActivated = false;
            predictor.Exit();

            activator.AnimationControl.SetUpperMask(false);

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

        protected virtual void PlayMarkerSound()
        {
            SoundManager.Instance.PlaySound(ConstStrings.SFX_DEFAULT_MARKER);
        }

        protected virtual void PlayThrowSound() { }

        public abstract IEnumerator ThrowObject(TrajectoryInfo info);
    }
}
