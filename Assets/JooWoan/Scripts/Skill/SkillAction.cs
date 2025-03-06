using System;
using EverScord.Character;
using UnityEngine;
using Photon.Pun;

namespace EverScord.Skill
{
    public abstract class SkillAction : MonoBehaviour
    {
        protected CharacterControl activator;
        protected CooldownTimer cooldownTimer;
        protected Coroutine skillCoroutine;
        protected PhotonView photonView;
        protected PlayerData.EJob ejob;
        protected int skillIndex;

        // int: skill index, float: cooldown duration
        public static Action<int, float> OnUsedSkill = delegate { };

        public virtual bool CanAttackWhileSkill
        {
            get { return false; }
        }

        public virtual bool IsUsingSkill
        {
            get { return skillCoroutine != null; }
        }

        private void Update()
        {
            UpdateCooldownProgress();
        }

        public virtual void Init(CharacterControl activator, CharacterSkill skill, PlayerData.EJob ejob, int skillIndex)
        {
            this.activator  = activator;
            this.skillIndex = skillIndex;
            this.ejob       = ejob;
            photonView      = activator.CharacterPhotonView;

            cooldownTimer = new CooldownTimer(skill.Cooldown);
            StartCoroutine(cooldownTimer.RunTimer());
        }

        public virtual bool Activate()
        {
            if (cooldownTimer.IsCooldown)
                return false;

            cooldownTimer.ResetElapsedTime();
            return true;
        }

        public virtual void ExitSkill()
        {
            if (skillCoroutine != null)
                StopCoroutine(skillCoroutine);
        }

        public abstract void OffensiveAction();
        public abstract void SupportAction();

        private void UpdateCooldownProgress()
        {
            if (!photonView.IsMine || !cooldownTimer.IsCooldown)
                return;

            OnUsedSkill?.Invoke(skillIndex, cooldownTimer.CooldownProgress);
        }
    }
}
