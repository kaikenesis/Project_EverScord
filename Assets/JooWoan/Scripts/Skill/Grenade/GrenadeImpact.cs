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
        private CharacterControl thrower;
        private bool isPhotonViewMine = false;

        void OnCollisionEnter(Collision collision)
        {
            if (((1 << collision.gameObject.layer) & skillAction.Skill.CollisionLayer) == 0)
                return;

            if (collision.transform.root == thrower.transform)
                return;

            var effect = Instantiate(explosionEffect, CharacterSkill.SkillRoot);
            effect.transform.position = transform.position;

            if (thrower.CharacterPhotonView.IsMine && skillAction != null && skillDelegate != null)
            {
                skillAction.SetGrenadeImpactPosition(transform.position);
                skillDelegate.Invoke();
            }
            
            Destroy(gameObject);
        }

        public void Init(CharacterControl activator, ThrowSkillAction skillAction)
        {
            this.skillAction = skillAction as GrenadeSkillAction;

            GrenadeSkill grenadeSkill = this.skillAction.Skill;
            thrower = activator;
            
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