using EverScord.Character;
using UnityEngine;
using Photon.Pun;

namespace EverScord.Skill
{
    public abstract class SkillAction : MonoBehaviour
    {
        protected CharacterControl activator;
        protected SkillTimer cooldownTimer;
        protected Coroutine skillCoroutine;
        protected PhotonView photonView;
        protected PlayerData.EJob ejob;
        protected int skillIndex;

        public CooldownTimer Timer => cooldownTimer;
        public CharacterSkillInfo SkillInfo { get; private set; }

        public virtual bool CanAttackWhileSkill
        {
            get { return false; }
        }

        public virtual bool IsUsingSkill
        {
            get { return skillCoroutine != null; }
        }

        void OnDestroy()
        {
            if (GameManager.IsInitialized)
                cooldownTimer.UnsubscribeOnCooldownOver(cooldownTimer.PlayCooldownUIEffect);
        }

        public virtual void Init(CharacterControl activator, CharacterSkill skill, PlayerData.EJob ejob, int skillIndex)
        {
            this.activator  = activator;
            this.skillIndex = skillIndex;
            this.ejob       = ejob;
            photonView      = activator.CharacterPhotonView;

            string tag      = ejob == PlayerData.EJob.Dealer ? skill.OffensiveTag : skill.SupportTag;
            SkillInfo       = SkillData.SkillInfoDict[tag];

            float cooldown  = activator.Stats.DecreasedCooldown(SkillInfo.cooldown);
            cooldownTimer   = new SkillTimer(cooldown, skillIndex, photonView.IsMine);
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

        public void SetNewCooldown()
        {
            float cooldown = activator.Stats.DecreasedCooldown(SkillInfo.cooldown);
            cooldownTimer.SetNewCooldown(cooldown);
        }

        public abstract void OffensiveAction();
        public abstract void SupportAction();
    }
}
