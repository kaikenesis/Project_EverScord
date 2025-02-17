using System;
using UnityEngine;
using EverScord.Character;

namespace EverScord.Skill
{
    public class GrenadeImpact : MonoBehaviour
    {
        private Action skillDelegate = null;
        private GrenadeSkillAction skillAction = null;
        private GameObject explosionEffect;

        void OnCollisionEnter(Collision collision)
        {
            if (skillAction == null)
                return;

            if (((1 << collision.gameObject.layer) & skillAction.Skill.CollisionLayer) == 0)
                return;

            var effect = Instantiate(explosionEffect, CharacterSkill.SkillRoot);
            effect.transform.position = transform.position;

            if (skillDelegate != null)
            {
                skillAction.SetGrenadeImpactPosition(transform.position);
                skillDelegate.Invoke();
            }
            
            Destroy(gameObject);
        }

        public void Init(CharacterControl activator, ThrowSkillAction skillAction)
        {
            if (!activator.CharacterPhotonView.IsMine)
                return;

            this.skillAction = skillAction as GrenadeSkillAction;

            if (!skillAction)
                return;

            GrenadeSkill grenadeSkill = this.skillAction.Skill;
            
            if (activator.CharacterJob == EJob.DEALER)
            {
                skillDelegate = skillAction.OffensiveAction;
                explosionEffect = ResourceManager.Instance.GetAsset<GameObject>(grenadeSkill.DamageEffectReference.AssetGUID);
            }
            else
            {
                skillDelegate = skillAction.SupportAction;
                explosionEffect = ResourceManager.Instance.GetAsset<GameObject>(grenadeSkill.HealEffectReference.AssetGUID);
            }
        }
    }
}