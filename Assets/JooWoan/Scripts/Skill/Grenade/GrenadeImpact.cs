using UnityEngine;

namespace EverScord.Skill
{
    public class GrenadeImpact : ThrowableImpact
    {
        private GameObject explosionEffect;

        public override void OnCollisionEnter(Collision collision)
        {
            if (!IsValidCollision(collision))
                return;

            GrenadeSkillAction grenadeSkillAction = skillAction as GrenadeSkillAction;
            GrenadeSkill grenadeSkill = grenadeSkillAction.Skill;

            if (thrower.CharacterJob == PlayerData.EJob.Dealer)
                explosionEffect = ResourceManager.Instance.GetAsset<GameObject>(grenadeSkill.DamageEffectReference.AssetGUID);
            else
                explosionEffect = ResourceManager.Instance.GetAsset<GameObject>(grenadeSkill.HealEffectReference.AssetGUID);

            var effect = Instantiate(explosionEffect, CharacterSkill.SkillRoot);
            effect.transform.position = transform.position;

            grenadeSkillAction.SetGrenadeImpactPosition(transform.position);
            onSkillActivated.Invoke();
            
            Destroy(gameObject);
        }
    }
}