using System.Collections;
using UnityEngine;
using EverScord.Character;
using EverScord.Effects;

namespace EverScord.Skill
{
    public class BombDeliveryAction : SkillAction
    {
        private const int MAX_COLLISION_CHECK = 100;
        private const float CHECK_DISTANCE_OFFSET = 0.5f;

        private BombDeliverySkill skill;
        private Transform closestTarget;
        private GameObject teleportEffect;

        public override void Init(CharacterControl activator, CharacterSkill skill, PlayerData.EJob ejob, int skillIndex)
        {
            base.Init(activator, skill, ejob, skillIndex);

            this.skill = (BombDeliverySkill)skill;
            teleportEffect = ResourceManager.Instance.GetAsset<GameObject>(AssetReferenceManager.TeleportEffect_ID);
        }

        public override bool Activate()
        {
            if (!base.Activate())
                return false;

            SoundManager.Instance.PlaySound(skill.ChargedSfx.AssetGUID);
            skillCoroutine = StartCoroutine(ActivateSkill());
            return true;
        }

        private IEnumerator ActivateSkill()
        {
            if (ejob == PlayerData.EJob.Dealer)
                OffensiveAction();
            else
                SupportAction();

            yield return new WaitForSeconds(0.1f);
            ExitSkill();
        }

        public override void OffensiveAction()
        {
            if (!SetClosestTarget(GameManager.EnemyLayer))
                return;

            TeleportPlayer();

            var explodeEffect = Instantiate(skill.BombPrefab, closestTarget);
            explodeEffect.transform.position = closestTarget.position;

            var impactEffect = Instantiate(skill.ImpactEffect, CharacterSkill.SkillRoot);
            impactEffect.transform.position = closestTarget.position;

            IEnemy enemy = closestTarget.GetComponent<IEnemy>();

            if (!activator.CharacterPhotonView.IsMine)
                return;

            float calculatedDamage = DamageCalculator.GetSkillDamage(activator, SkillInfo.skillDamage, SkillInfo.skillCoefficient, enemy);
            GameManager.Instance.EnemyHitsControl.ApplyDamageToEnemy(activator, calculatedDamage, enemy);

            enemy.StunMonster(skill.StunDuration);
            SoundManager.Instance.PlaySound(skill.BombSfx.AssetGUID);
        }

        public override void SupportAction()
        {
            if (!SetClosestTarget(GameManager.PlayerLayer))
                return;

            TeleportPlayer();
            CharacterControl target = closestTarget.GetComponent<CharacterControl>();

            var healEffect = Instantiate(skill.HealCircleEffect, CharacterSkill.SkillRoot);
            healEffect.transform.position = closestTarget.position;

            if (!activator.CharacterPhotonView.IsMine)
                return;

            float totalHealAmount = DamageCalculator.GetSkillDamage(activator, SkillInfo.skillDamage, SkillInfo.skillCoefficient);
            float dotHealAmount = DamageCalculator.GetSkillDamage(activator, SkillInfo.skillDotDamage, SkillInfo.skillCoefficient);

            target.IncreaseHP(activator, totalHealAmount, true);
            StartCoroutine(CharacterSkill.RegenerateHP(activator, new CharacterControl[] {target}, skill.HealDuration, dotHealAmount));
            SoundManager.Instance.PlaySound(skill.HealSfx.AssetGUID);
        }

        private void TeleportPlayer()
        {
            SoundManager.Instance.PlaySound(ConstStrings.SFX_TELEPORT);

            var effect1 = Instantiate(teleportEffect, CharacterSkill.SkillRoot);
            var effect2 = Instantiate(teleportEffect, CharacterSkill.SkillRoot);

            Vector3 effectScale = effect1.transform.localScale;
            effect1.transform.localScale = new Vector3(effectScale.x * 0.5f, effectScale.y * 0.5f, effectScale.z * 0.5f);

            effect1.transform.position   = activator.PlayerTransform.position;
            activator.transform.position = GetSafeTeleportPosition(closestTarget.position);
            effect2.transform.position   = activator.PlayerTransform.position;
        }

        private bool SetClosestTarget(LayerMask layerMask)
        {
            Collider[] colliders = Physics.OverlapSphere(activator.PlayerTransform.position, SkillInfo.skillRange, layerMask);

            var electricEffect = Instantiate(skill.TeleportElectric, activator.PlayerTransform);
            electricEffect.transform.position = activator.PlayerTransform.position;

            if (colliders.Length <= 0)
                return false;

            closestTarget = null;
            float closestDistance = -1;

            bool isPlayerLayer = layerMask == GameManager.PlayerLayer;

            for (int i = 0; i < colliders.Length; i++)
            {
                float distance = Vector3.Distance(activator.PlayerTransform.position, colliders[i].transform.position);

                if (!closestTarget || (closestDistance > distance))
                {
                    // Exclude myself
                    if (isPlayerLayer && colliders[i].transform.root == activator.PlayerTransform)
                        continue;
                    
                    closestDistance = distance;
                    closestTarget = colliders[i].transform;
                }
            }

            return closestTarget != null;
        }

        private Vector3 GetSafeTeleportPosition(Vector3 targetPos)
        {
            Vector3 playerToTargetDir = (targetPos - activator.PlayerTransform.position).normalized;

            Vector3 teleportPos = targetPos;
            float checkRadius = activator.Controller.radius;

            int checkCount = 0;
            float distance = 1f;

            while (Physics.CheckSphere(teleportPos, checkRadius, skill.CollidableLayer) && checkCount <= MAX_COLLISION_CHECK)
            {
                ++checkCount;
                distance += CHECK_DISTANCE_OFFSET;
                teleportPos = targetPos + playerToTargetDir * distance;
            }

            teleportPos.y = 0f;
            return teleportPos;
        }

        public override void ExitSkill()
        {
            skillCoroutine = null;
        }
    }
}
