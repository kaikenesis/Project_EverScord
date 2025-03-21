using System.Collections;
using UnityEngine;
using EverScord.Character;
using EverScord.Effects;
using EverScord.Pool;

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
        private GameObject bomb, healCircle;
        private WaitForSeconds waitStrikeInterval, waitBombDrop, waitZoneDuration;
        private AircraftControl aircraft1, aircraft2, aircraft3;

        public override void Init(CharacterControl activator, CharacterSkill skill, PlayerData.EJob ejob, int skillIndex)
        {
            base.Init(activator, skill, ejob, skillIndex);
            Skill = (AirStrikeSkill)skill;

            waitStrikeInterval  = new WaitForSeconds(Skill.ExplosionInterval);
            waitZoneDuration    = new WaitForSeconds(Skill.ZoneDuration);
            waitBombDrop        = new WaitForSeconds(0.3f);

            aircraft1 = Instantiate(Skill.AircraftPrefab, CharacterSkill.SkillRoot).GetComponent<AircraftControl>();
            aircraft2 = Instantiate(Skill.AircraftPrefab, CharacterSkill.SkillRoot).GetComponent<AircraftControl>();
            aircraft3 = Instantiate(Skill.AircraftPrefab, CharacterSkill.SkillRoot).GetComponent<AircraftControl>();

            aircraft1.Init(Skill.AirCraftTravelDistance, Skill.AirCraftSpeed, Skill.Jet1Sfx.AssetGUID);
            aircraft2.Init(Skill.AirCraftTravelDistance, Skill.AirCraftSpeed * AIRCRAFT_2_SPEED, Skill.Jet2Sfx.AssetGUID, AIRCRAFT_2_DELAY);
            aircraft3.Init(Skill.AirCraftTravelDistance, Skill.AirCraftSpeed * AIRCRAFT_3_SPEED, Skill.Jet3Sfx.AssetGUID, AIRCRAFT_3_DELAY);

            if (ejob == PlayerData.EJob.Dealer)
            {
                targetLayer = GameManager.EnemyLayer;
                bomb = ResourceManager.Instance.GetAsset<GameObject>(Skill.BombEffectReference.AssetGUID);
                _ = ResourceManager.Instance.CreatePool(Skill.ExplosionEffectReference.AssetGUID, 5);
            }
            else
            {
                targetLayer = GameManager.PlayerLayer;
                healCircle = ResourceManager.Instance.GetAsset<GameObject>(Skill.HealZoneEffectReference.AssetGUID);
                _ = ResourceManager.Instance.CreatePool(Skill.HealExplosionReference.AssetGUID, 5);
            }
        }

        protected override void PlayThrowSound()
        {
            SoundManager.Instance.PlaySound(Skill.ThrowSfx.AssetGUID);
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

            SoundManager.Instance.PlaySound(Skill.BombingSfx.AssetGUID);
            SoundManager.Instance.PlaySound(Skill.ImpactSfx.AssetGUID);

            for (int i = 0; i < Skill.BombCount; i++, distanceSum += dropDistance)
            {
                Vector3 dropPosition = strikeStartPos + direction * distanceSum;
                PoolableVfx explosion = null;

                if (ejob == PlayerData.EJob.Dealer)
                {
                    explosion = ResourceManager.Instance.GetFromPool(Skill.ExplosionEffectReference.AssetGUID) as PoolableVfx;

                    var effect = Instantiate(bomb, CharacterSkill.SkillRoot);
                    effect.transform.position = dropPosition;
                }
                else
                    explosion = ResourceManager.Instance.GetFromPool(Skill.HealExplosionReference.AssetGUID) as PoolableVfx;

                if (explosion)
                {
                    explosion.transform.position = dropPosition;
                    explosion.Play(0.3f);
                }

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
                    float bombDamage = DamageCalculator.GetSkillDamage(activator, SkillInfo.skillDamage, SkillInfo.skillCoefficient, enemy);
                    GameManager.Instance.EnemyHitsControl.ApplyDamageToEnemy(activator, bombDamage, enemy);
                }
                else
                {
                    CharacterControl player = colliders[i].GetComponent<CharacterControl>();
                    float bombHeal = DamageCalculator.GetSkillDamage(activator, SkillInfo.skillDamage, SkillInfo.skillCoefficient);
                    player.IncreaseHP(activator, bombHeal, true);
                }
            }
        }

        private IEnumerator CreateZone(Vector3 dropPosition)
        {
            yield return waitBombDrop;
            
            PoolableVfx flames = null;
            GameObject healEffect = null;

            if (ejob == PlayerData.EJob.Dealer)
            {
                flames = ResourceManager.Instance.GetFromPool(Skill.FlameEffectReference.AssetGUID) as PoolableVfx;

                flames.transform.position = new Vector3(
                    dropPosition.x,
                    flames.transform.position.y,
                    dropPosition.z
                );

                SoundManager.Instance.PlaySound(Skill.FlameSfx.AssetGUID);
                flames.SetDuration(Skill.ZoneDuration + 2);
                flames.Play();
            }
            else
            {
                SoundManager.Instance.PlaySound(Skill.HealSfx.AssetGUID);
                healEffect = Instantiate(healCircle, CharacterSkill.SkillRoot);
                healEffect.transform.position = dropPosition;

            }

            if (photonView.IsMine)
            {
                if (ejob == PlayerData.EJob.Dealer && flames)
                {
                    FlameControl flameControl = flames.GetComponent<FlameControl>();

                    flameControl.Init(
                        activator,
                        Skill.ZoneInfluenceInterval,
                        SkillInfo,
                        targetLayer
                    );
                }
                else
                {
                    HealZoneControl healZoneControl = healEffect.GetComponent<HealZoneControl>();

                    healZoneControl.Init(
                        activator,
                        Skill.ZoneInfluenceInterval,
                        SkillInfo,
                        targetLayer
                    );
                }
            }

            yield return waitZoneDuration;

            if (flames)
                flames.Stop();

            SoundManager.Instance.StopSound(Skill.FlameSfx.AssetGUID);
            SoundManager.Instance.StopSound(Skill.HealSfx.AssetGUID);

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