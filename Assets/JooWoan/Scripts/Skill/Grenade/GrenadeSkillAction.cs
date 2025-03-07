using System.Collections;
using UnityEngine;
using EverScord.Character;

namespace EverScord.Skill
{
    public class GrenadeSkillAction : ThrowSkillAction
    {
        public GrenadeSkill Skill { get; private set; }
        private Vector3 grenadeImpactPosition;
        
        public override void Init(CharacterControl activator, CharacterSkill skill, PlayerData.EJob ejob, int skillIndex)
        {            
            Skill = (GrenadeSkill)skill;

            base.Init(activator, skill, ejob, skillIndex);

            GameObject mainMarker = predictor.MarkerControl.Marker.gameObject;
            GameObject stampMarker = predictor.MarkerControl.StampedMarker.gameObject;
            
            SkillMarker.SetMarkerColor(mainMarker, Skill.MarkerColor);
            SkillMarker.SetMarkerColor(stampMarker, Skill.StampMarkerColor);
        }

        public override void OffensiveAction()
        {
            if (!photonView.IsMine)
                return;

            Collider[] colliders = Physics.OverlapSphere(grenadeImpactPosition, Skill.ExplosionRadius, GameManager.EnemyLayer);
            float calculatedDamage = DamageCalculator.GetSkillDamage(activator, Skill);

            for (int i = 0; i < colliders.Length; i++)
            {
                IEnemy enemy = colliders[i].GetComponent<IEnemy>();
                GameManager.Instance.EnemyHitsControl.ApplyDamageToEnemy(calculatedDamage, enemy);

                if (enemy is BossRPC boss)
                    boss.SetDebuff(EBossDebuff.POISON, Skill.PoisonedDuration, Skill.PoisonDamage);
            }
        }

        public override void SupportAction()
        {
            if (!photonView.IsMine)
                return;
            
            Collider[] colliders = Physics.OverlapSphere(grenadeImpactPosition, Skill.ExplosionRadius, GameManager.PlayerLayer);
            float calculatedHeal = Skill.BaseHeal;

            for (int i = 0; i < colliders.Length; i++)
            {
                CharacterControl player = colliders[i].GetComponent<CharacterControl>();
                player.IncreaseHP(calculatedHeal, true);
            }
        }

        public void SetGrenadeImpactPosition(Vector3 position)
        {
            grenadeImpactPosition = position;
        }

        public override IEnumerator ThrowObject(TrajectoryInfo info)
        {
            Transform grenade = Instantiate(Skill.ThrowingObject, CharacterSkill.SkillRoot).transform;
            grenade.GetComponent<ThrowableImpact>().Init(activator, this, info);

            StartCoroutine(predictor.ThrowObject(grenade, info));
            yield break;
        }
    }
}
