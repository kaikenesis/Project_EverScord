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
        protected EJob ejob;
        protected int skillIndex;

        protected bool isOffensive => ejob == EJob.DEALER;
        public virtual bool CanAttackWhileSkill
        {
            get { return false; }
        }

        public virtual bool IsUsingSkill
        {
            get { return skillCoroutine != null; }
        }

        public virtual void Init(CharacterControl activator, CharacterSkill skill, EJob ejob, int skillIndex)
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

        public abstract void OffensiveAction();
        public abstract void SupportAction();
    }
}
