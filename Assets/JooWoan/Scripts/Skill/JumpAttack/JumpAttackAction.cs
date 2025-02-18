using System.Collections;
using UnityEngine;
using EverScord.Character;

using AnimationInfo = EverScord.Character.AnimationInfo;
using Photon.Pun;

namespace EverScord.Skill
{
    public class JumpAttackAction : SkillAction
    {
        public JumpAttackSkill Skill { get; private set; }

        private CharacterAnimation animControl;
        private AnimationInfo animInfo;
        private WaitForSeconds waitSkill;
        private Coroutine effectCoroutine, counterCoroutine;
        private GameObject stanceEffect;
        private SkillMarker skillMarker;

        private bool canJump = false;

        public override void Init(CharacterControl activator, CharacterSkill skill, EJob ejob, int skillIndex)
        {
            Skill       = (JumpAttackSkill)skill;
            waitSkill   = new WaitForSeconds(Skill.Duration);
            skillMarker = new SkillMarker(activator, transform, Skill.Marker);

            animControl = activator.AnimationControl;
            animInfo    = animControl.AnimInfo;

            base.Init(activator, skill, ejob, skillIndex);
        }

        public override bool Activate()
        {
            if (cooldownTimer.IsCooldown)
                return false;

            activator.SetState(SetCharacterStateMode.ADD, CharacterState.SKILL_STANCE);
            skillCoroutine = StartCoroutine(ActivateSkill());
            return true;
        }

        private IEnumerator ActivateSkill()
        {
            stanceEffect = Instantiate(Skill.StanceEffect, CharacterSkill.SkillRoot);
            stanceEffect.transform.position = activator.transform.position;

            animControl.CrossFade(animInfo.CounterStance, 0.1f);
            activator.SetCharacterOutline(true);

            if (counterCoroutine != null)
                StopCoroutine(counterCoroutine);

            counterCoroutine = StartCoroutine(CounterStance());

            yield return waitSkill;
            ExitSkill();
        }

        private IEnumerator CounterStance()
        {
            skillMarker.Set(true);

            while (true)
            {
                if (canJump && activator.PlayerInputInfo.pressedLeftMouseButton)
                {
                    canJump = false;
                    activator.SetMouseButtonDown(false);
                    cooldownTimer.ResetElapsedTime();

                    skillMarker.Set(false);

                    //StartCoroutine(Jump());
                    SendRPC();
                    ExitSkill();

                    yield break;
                }

                yield return null;
            }
        }

        private void ExitSkill()
        {
            CharacterSkill.StopEffectParticles(stanceEffect);
            activator.SetState(SetCharacterStateMode.REMOVE, CharacterState.SKILL_STANCE);

            animControl.CrossFade(animInfo.Idle, 0.3f);
            activator.SetCharacterOutline(false);

            skillCoroutine = null;
        }

        //private IEnumerator Jump()
        //{
        //    skillMarker.SetStamped(true);
        //}

        public override void OffensiveAction()
        {
            throw new System.NotImplementedException();
        }

        public override void SupportAction()
        {
            throw new System.NotImplementedException();
        }

        public void EnableJump()
        {
            canJump = true;
        }

        private void SendRPC()
        {
            if (!PhotonNetwork.IsConnected)
                return;

            photonView.RPC(nameof(activator.SyncThrowSkill), RpcTarget.Others, activator.MouseRayHitPos);
        }
    }
}