using System.Collections;
using UnityEngine;
using EverScord.Character;

namespace EverScord.Skill
{
    public class GrenadeSkillAction : ThrowSkillAction
    {
        public GrenadeSkill Skill { get; private set; }
        private GameObject grenade;
        private Vector3 grenadeImpactPosition;
        
        public override void Init(CharacterControl activator, CharacterSkill skill, EJob ejob, int skillIndex)
        {            
            Skill = (GrenadeSkill)skill;

            if (ejob == EJob.DEALER)
                grenade = Skill.PoisonBomb;

            else if (ejob == EJob.HEALER)
                grenade = Skill.HealBomb;

            base.Init(activator, skill, ejob, skillIndex);
            TrajectoryPredictor.SetStampMarkerColor(predictor.StampedMarker, Skill.StampMarkerColor);
        }

        public override void OffensiveAction()
        {
            Collider[] colliders = Physics.OverlapSphere(grenadeImpactPosition, Skill.ExplosionRadius, GameManager.EnemyLayer);
            float calculatedDamage = DamageCalculator.GetSkillDamage(activator, Skill);

            for (int i = 0; i < colliders.Length; i++)
            {
                IEnemy enemy = colliders[i].GetComponent<IEnemy>();
                GameManager.Instance.EnemyHitsControl.ApplyDamageToEnemy(calculatedDamage, enemy);
            }
        }

        public override void SupportAction()
        {
            Collider[] colliders = Physics.OverlapSphere(grenadeImpactPosition, Skill.ExplosionRadius, GameManager.PlayerLayer);
            float calculatedHeal = Skill.BaseHeal;

            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].transform.root == activator.transform)
                    continue;

                CharacterControl player = colliders[i].GetComponent<CharacterControl>();
                player.IncreaseHP(calculatedHeal);
            }
        }

        public void SetGrenadeImpactPosition(Vector3 position)
        {
            grenadeImpactPosition = position;
        }

        public override IEnumerator ThrowObject()
        {
            Transform throwingObject = Instantiate(grenade, CharacterSkill.SkillRoot).transform;
            throwingObject.GetComponent<GrenadeImpact>().Init(activator, this);

            StartCoroutine(predictor.ThrowObject(throwingObject));
            yield break;
        }
    }
}
