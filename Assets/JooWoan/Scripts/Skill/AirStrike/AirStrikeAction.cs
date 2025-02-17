using System.Collections;
using UnityEngine;
using EverScord.Character;

namespace EverScord.Skill
{
    public class AirStrikeAction : ThrowSkillAction
    {
        public AirStrikeSkill Skill { get; private set; }

        private Vector3 strikeStartPos, strikeEndPos;
        private LayerMask targetLayer;
        private GameObject bomb, flames, healCircle;
        private WaitForSeconds waitStrikeInterval, waitBombDrop, waitFlameDuration;
        private AircraftControl aircraft;
        private float calculatedImpact;
        private bool isOffensive;

        public override void Init(CharacterControl activator, CharacterSkill skill, EJob ejob, int skillIndex)
        {            
            Skill = (AirStrikeSkill)skill;

            waitStrikeInterval  = new WaitForSeconds(Skill.ExplosionInterval);
            waitFlameDuration   = new WaitForSeconds(Skill.FlameDuration);
            waitBombDrop        = new WaitForSeconds(0.1f);

            isOffensive = activator.CharacterJob == EJob.DEALER;
            
            aircraft = Instantiate(Skill.AircraftPrefab).GetComponent<AircraftControl>();
            aircraft.Init(Skill.AirCraftTravelDistance, Skill.AirCraftSpeed);
            
            if (isOffensive)
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

                bomb = ResourceManager.Instance.GetAsset<GameObject>(Skill.BombEffectReference.AssetGUID);
                healCircle = ResourceManager.Instance.GetAsset<GameObject>(Skill.HealEffectReference.AssetGUID);
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

            aircraft.EnableAircraft(strikeStartPos, direction);

            for (int i = 0; i < Skill.BombCount; i++, distanceSum += dropDistance)
            {
                Vector3 dropPosition = strikeStartPos + direction * distanceSum;

                var effect = Instantiate(bomb, CharacterSkill.SkillRoot);
                effect.transform.position = dropPosition;

                if (photonView.IsMine)
                    StartCoroutine(ProceedCollisionCheck(dropPosition));

                if (isOffensive)
                    StartCoroutine(CreateFlames(dropPosition));

                yield return waitStrikeInterval;
            }
        }

        private IEnumerator ProceedCollisionCheck(Vector3 dropPosition)
        {
            yield return waitBombDrop;

            Collider[] colliders = Physics.OverlapSphere(dropPosition, Skill.ExplosionRadius, targetLayer);

            for (int i = 0; i < colliders.Length; i++)
            {
                if (isOffensive)
                {
                    IEnemy enemy = colliders[i].GetComponent<IEnemy>();
                    GameManager.Instance.EnemyHitsControl.ApplyDamageToEnemy(calculatedImpact, enemy);
                }
                else if (colliders[i].transform.root != activator.transform)
                {
                    CharacterControl player = colliders[i].GetComponent<CharacterControl>();
                    player.IncreaseHP(calculatedImpact);
                }
            }
        }

        private IEnumerator CreateFlames(Vector3 dropPosition)
        {
            yield return waitBombDrop;
            
            GameObject flameEffect = Instantiate(flames, CharacterSkill.SkillRoot);
            flameEffect.transform.position = dropPosition;

            FlameControl flameControl = null;

            if (photonView.IsMine)
            {
                flameControl = flameEffect.GetComponent<FlameControl>();

                flameControl.Init(
                    GameManager.Instance.EnemyHitsControl.ApplyDamageToEnemy,
                    Skill.FlameHurtInterval,
                    Skill.FlameBaseDamage,
                    targetLayer
                );
            }

            yield return waitFlameDuration;

            CharacterSkill.StopEffectParticles(flameEffect);
        }

        public override IEnumerator ThrowObject()
        {
            GameObject flare = Instantiate(ThrowingSkill.ThrowingObject, CharacterSkill.SkillRoot);
            flare.GetComponent<ThrowableImpact>().Init(activator, this);

            StartCoroutine(predictor.ThrowObject(flare.transform));
            yield break;
        }

        public void SetAirStrikePosition(Vector3 startPos, Vector3 endPos)
        {
            strikeStartPos = startPos;
            strikeEndPos = endPos;
        }
    }
}