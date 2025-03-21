using EverScord.Pool;
using UnityEngine;

namespace EverScord.Skill
{
    public class GrenadeImpact : ThrowableImpact
    {
        private PoolableVfx explosionEffect;

        protected override void Impact()
        {
            GrenadeSkillAction grenadeSkillAction = skillAction as GrenadeSkillAction;
            GrenadeSkill grenadeSkill = grenadeSkillAction.Skill;

            if (thrower.CharacterJob == PlayerData.EJob.Dealer)
                explosionEffect = ResourceManager.Instance.GetFromPool(grenadeSkill.DamageEffectReference.AssetGUID) as PoolableVfx;
            else
                explosionEffect = ResourceManager.Instance.GetFromPool(grenadeSkill.HealEffectReference.AssetGUID) as PoolableVfx;

            explosionEffect.transform.position = new Vector3(
                transform.position.x,
                explosionEffect.transform.position.y,
                transform.position.z
            );

            explosionEffect.Play();
            SoundManager.Instance.PlaySound(grenadeSkill.GrenadeExplodeSfx.AssetGUID);

            grenadeSkillAction.SetGrenadeImpactPosition(transform.position);
            onSkillActivated.Invoke();
        }
    }
}