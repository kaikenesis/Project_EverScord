using System.Collections;
using UnityEngine;
using EverScord.Character;
using EverScord.Effects;

namespace EverScord.Skill
{
    public class AirStrikeAction : ThrowSkillAction
    {
        private const float AIRCRAFT_2_SPEED = 1.3f;
        private const float AIRCRAFT_3_SPEED = 1.5f;
        private const float AIRCRAFT_2_DELAY = 0.5f;
        private const float AIRCRAFT_3_DELAY = 1.1f;
        private const float AIRCRAFT_DISTANCE = 4.5f;
        public AirStrikeSkill Skill { get; private set; }

        private Vector3 strikeStartPos, strikeEndPos;
        private LayerMask targetLayer;
        private GameObject bomb, flames, healCircle;
        private WaitForSeconds waitStrikeInterval, waitBombDrop, waitZoneDuration;
        private AircraftControl aircraft1, aircraft2, aircraft3;
        private float calculatedImpact;

        public override void Init(CharacterControl activator, CharacterSkill skill, PlayerData.EJob ejob, int skillIndex)
        {
            Skill = (AirStrikeSkill)skill;

            waitStrikeInterval  = new WaitForSeconds(Skill.ExplosionInterval);
            waitZoneDuration    = new WaitForSeconds(Skill.ZoneDuration);
            waitBombDrop        = new WaitForSeconds(0.3f);

            aircraft1 = Instantiate(Skill.AircraftPrefab, CharacterSkill.SkillRoot).GetComponent<AircraftControl>();
            aircraft2 = Instantiate(Skill.AircraftPrefab, CharacterSkill.SkillRoot).GetComponent<AircraftControl>();
            aircraft3 = Instantiate(Skill.AircraftPrefab, CharacterSkill.SkillRoot).GetComponent<AircraftControl>();

            aircraft1.Init(Skill.AirCraftTravelDistance, Skill.AirCraftSpeed);
            aircraft2.Init(Skill.AirCraftTravelDistance, Skill.AirCraftSpeed * AIRCRAFT_2_SPEED, AIRCRAFT_2_DELAY);
            aircraft3.Init(Skill.AirCraftTravelDistance, Skill.AirCraftSpeed * AIRCRAFT_3_SPEED, AIRCRAFT_3_DELAY);

            if (ejob == PlayerData.EJob.Dealer)
            {
                targetLayer = GameManager.EnemyLayer;
                calculatedImpact = DamageCalculator.GetSkillDamage(activator, Skill);
                bomb = ResourceManager.Instance.GetAsset<GameObject>(Skill.BombEffectReference.AssetGUID);
                flames = ResourceManager.Instance.GetAsset<GameObject>(Skill.FlameEffectReference.AssetGUID);
            }
            else
            {
                targetLayer = GameManager.PlayerLayer;

                // Calculate total heal amount
                calculatedImpact = Skill.BaseHeal;
                bomb = ResourceManager.Instance.GetAsset<GameObject>(Skill.HealBombEffectReference.AssetGUID);
                healCircle = ResourceManager.Instance.GetAsset<GameObject>(Skill.HealZoneEffectReference.AssetGUID);
            }

            base.Init(activator, skill, ejob, skillIndex);
        }

        public override void OffensiveAction()
        {
            StartCoroutine(DropBombs());
        }

        public override void SupportAction()
        {
            StartCoroutine(DropBombs());
        }

        private IEnumerator DropBombs()
        {
            Vector3 direction = (strikeEndPos - strikeStartPos).normalized;
            float dropDistance = Vector3.Distance(strikeStartPos, strikeEndPos) / Mathf.Max(1, Skill.BombCount - 1);
            float distanceSum = 0f;

            Vector3 leftAircraftDir  = Quaternion.Euler(0, -90, 0) * direction;
            Vector3 rightAirCraftDir = Quaternion.Euler(0, 90, 0) * direction;

            Vector3 leftAircraftPos  = strikeStartPos + leftAircraftDir * AIRCRAFT_DISTANCE;
            Vector3 rightAircraftPos = strikeStartPos + rightAirCraftDir * AIRCRAFT_DISTANCE;

            aircraft1.EnableAircraft(strikeStartPos, direction);
            aircraft2.EnableAircraft(leftAircraftPos, direction);
            aircraft3.EnableAircraft(rightAircraftPos, direction);

            for (int i = 0; i < Skill.BombCount; i++, distanceSum += dropDistance)
            {
                Vector3 dropPosition = strikeStartPos + direction * distanceSum;

                var effect = Instantiate(bomb, CharacterSkill.SkillRoot);
                effect.transform.position = dropPosition;

                if (photonView.IsMine)
                    StartCoroutine(ProceedCollisionCheck(dropPosition));

                StartCoroutine(CreateZone(dropPosition));
                yield return waitStrikeInterval;
            }
        }

        private IEnumerator ProceedCollisionCheck(Vector3 dropPosition)
        {
            yield return waitBombDrop;

            Collider[] colliders = Physics.OverlapSphere(dropPosition, Skill.ExplosionRadius, targetLayer);

            for (int i = 0; i < colliders.Length; i++)
            {
                if (ejob == PlayerData.EJob.Dealer)
                {
                    IEnemy enemy = colliders[i].GetComponent<IEnemy>();
                    GameManager.Instance.EnemyHitsControl.ApplyDamageToEnemy(calculatedImpact, enemy);
                }
                else
                {
                    CharacterControl player = colliders[i].GetComponent<CharacterControl>();
                    player.IncreaseHP(calculatedImpact, true);
                }
            }
        }

        private IEnumerator CreateZone(Vector3 dropPosition)
        {
            yield return waitBombDrop;
            
            FlameControl flameControl = null;
            HealZoneControl healZoneControl = null;

            GameObject flameEffect = null;
            GameObject healEffect = null;

            if (ejob == PlayerData.EJob.Dealer)
            {
                flameEffect = Instantiate(flames, CharacterSkill.SkillRoot);
                flameEffect.transform.position = dropPosition;
            }
            else
            {
                healEffect = Instantiate(healCircle, CharacterSkill.SkillRoot);
                healEffect.transform.position = dropPosition;
            }

            if (photonView.IsMine)
            {
                if (ejob == PlayerData.EJob.Dealer)
                {
                    flameControl = flameEffect.GetComponent<FlameControl>();

                    flameControl.Init(
                        GameManager.Instance.EnemyHitsControl.ApplyDamageToEnemy,
                        Skill.ZoneInfluenceInterval,
                        Skill.FlameBaseDamage,
                        targetLayer
                    );
                }
                else
                {
                    healZoneControl = healEffect.GetComponent<HealZoneControl>();

                    healZoneControl.Init(
                        Skill.ZoneInfluenceInterval,
                        Skill.HealBaseAmount,
                        targetLayer
                    );
                }
            }

            yield return waitZoneDuration;

            EffectControl.SetEffectParticles(flameEffect, false);
            EffectControl.SetEffectParticles(healEffect, false);
        }

        public override IEnumerator ThrowObject(TrajectoryInfo info)
        {
            GameObject flare = Instantiate(ThrowingSkill.ThrowingObject, CharacterSkill.SkillRoot);
            flare.GetComponent<ThrowableImpact>().Init(activator, this, info);

            StartCoroutine(predictor.ThrowObject(flare.transform, info));
            yield break;
        }

        public void SetAirStrikePosition(Vector3 startPos, Vector3 endPos)
        {
            strikeStartPos = startPos;
            strikeEndPos = endPos;
        }
    }
}