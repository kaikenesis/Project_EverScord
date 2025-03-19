using System.Collections;
using UnityEngine;
using EverScord.Character;

namespace EverScord.Skill
{
    public class GrenadeSkillAction : ThrowSkillAction
    {
        public GrenadeSkill Skill { get; private set; }
        private Vector3 grenadeImpactPosition;
        private float skillRadius;
        
        public override void Init(CharacterControl activator, CharacterSkill skill, PlayerData.EJob ejob, int skillIndex)
        {            
            base.Init(activator, skill, ejob, skillIndex);
            Skill = (GrenadeSkill)skill;
            skillRadius = SkillInfo.skillSizes[0];

            GameObject mainMarker = predictor.MarkerControl.Marker.gameObject;
            GameObject stampMarker = predictor.MarkerControl.StampedMarker.gameObject;
            
            SkillMarker.SetMarkerColor(mainMarker, Skill.MarkerColor);
            SkillMarker.SetMarkerColor(stampMarker, Skill.StampMarkerColor);
        }

        public override void OffensiveAction()
        {
            if (!photonView.IsMine)
                return;

            Collider[] colliders = Physics.OverlapSphere(grenadeImpactPosition, skillRadius, GameManager.EnemyLayer);

            for (int i = 0; i < colliders.Length; i++)
            {
                IEnemy enemy = colliders[i].GetComponent<IEnemy>();

                float calculatedDamage = DamageCalculator.GetSkillDamage(activator, SkillInfo.skillDamage, SkillInfo.skillCoefficient, enemy);
                GameManager.Instance.EnemyHitsControl.ApplyDamageToEnemy(activator, calculatedDamage, enemy);
                enemy.SetDebuff(activator, EBossDebuff.POISON, Skill.PoisonedDuration, SkillInfo.skillDotDamage);
            }
        }

        public override void SupportAction()
        {
            if (!photonView.IsMine)
                return;

            Collider[] colliders = Physics.OverlapSphere(grenadeImpactPosition, skillRadius, GameManager.PlayerLayer);
            var players = new CharacterControl[colliders.Length];

            float totalHealAmount = DamageCalculator.GetSkillDamage(activator, SkillInfo.skillDamage, SkillInfo.skillCoefficient);
            float dotHealAmount = DamageCalculator.GetSkillDamage(activator, SkillInfo.skillDotDamage, SkillInfo.skillCoefficient);

            for (int i = 0; i < colliders.Length; i++)
            {
                players[i] = colliders[i].GetComponent<CharacterControl>();
                players[i].IncreaseHP(activator, totalHealAmount, true);
            }

            StartCoroutine(CharacterSkill.RegenerateHP(activator, players, Skill.HealDuration, dotHealAmount));
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
