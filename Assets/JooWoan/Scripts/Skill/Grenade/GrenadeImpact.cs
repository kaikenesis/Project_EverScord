using UnityEngine;

namespace EverScord.Skill
{
    public class GrenadeImpact : ThrowableImpact
    {
        private GameObject explosionEffect;

        protected override void Impact()
        {
            GrenadeSkillAction grenadeSkillAction = skillAction as GrenadeSkillAction;
            GrenadeSkill grenadeSkill = grenadeSkillAction.Skill;

            if (thrower.CharacterJob == EJob.DEALER)
                explosionEffect = ResourceManager.Instance.GetAsset<GameObject>(grenadeSkill.DamageEffectReference.AssetGUID);
            else
                explosionEffect = ResourceManager.Instance.GetAsset<GameObject>(grenadeSkill.HealEffectReference.AssetGUID);

            var effect = Instantiate(explosionEffect, CharacterSkill.SkillRoot);
            effect.transform.position = transform.position;

            grenadeSkillAction.SetGrenadeImpactPosition(transform.position);
            onSkillActivated.Invoke();
        }
    }
}