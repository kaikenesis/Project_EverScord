using System.Collections;
using UnityEngine;
using EverScord.Character;
using System;

namespace EverScord.Skill
{
    public class BombDeliveryAction : MonoBehaviour, ISkillAction
    {
        private const int MAX_COLLISION_CHECK = 100;
        private const float CHECK_DISTANCE_OFFSET = 0.5f;

        private CharacterControl activator;
        private BombDeliverySkill skill;
        private CooldownTimer cooldownTimer;
        private Coroutine skillCoroutine;

        private EJob ejob;
        private int skillIndex;

        public bool IsUsingSkill { get { return skillCoroutine != null; } }
        public bool CanAttackWhileSkill => false;

        public void Init(CharacterControl activator, CharacterSkill skill, EJob ejob, int skillIndex)
        {
            this.activator  = activator;
            this.skill      = (BombDeliverySkill)skill;
            this.skillIndex = skillIndex;
            this.ejob       = ejob;

            cooldownTimer = new CooldownTimer(skill.Cooldown);
            StartCoroutine(cooldownTimer.RunTimer());
        }

        public void Activate()
        {
            if (cooldownTimer.IsCooldown || IsUsingSkill)
                return;

            skillCoroutine = StartCoroutine(ActivateSkill());
        }

        private IEnumerator ActivateSkill()
        {
            Collider[] colliders = Physics.OverlapSphere(activator.transform.position, skill.DetectRadius, skill.DetectLayer);

            if (colliders.Length <= 0)
                yield break;

            Collider closest = colliders[0];
            float closestDistance = Vector3.Distance(activator.transform.position, colliders[0].transform.position);

            for (int i = 1; i < colliders.Length; i++)
            {
                float distance = Vector3.Distance(activator.transform.position, colliders[i].transform.position);

                if (closestDistance > distance)
                {
                    closestDistance = distance;
                    closest = colliders[i];
                }
            }

            var effect1 = Instantiate(skill.TeleportEffect, CharacterSkill.SkillRoot);
            var effect2 = Instantiate(skill.TeleportEffect, CharacterSkill.SkillRoot);

            Vector3 effectScale = effect1.transform.localScale;
            effect1.transform.localScale = new Vector3(effectScale.x * 0.5f, effectScale.y * 0.5f, effectScale.z * 0.5f);

            effect1.transform.position = activator.transform.position;

            activator.transform.position = GetSafeTeleportPosition(closest);

            effect2.transform.position = activator.transform.position;
        }

        private Vector3 GetSafeTeleportPosition(Collider enemyCol)
        {
            Vector3 enemyPos = enemyCol.transform.position;
            Vector3 playerToEnemyDir = (enemyPos - activator.transform.position).normalized;

            Vector3 teleportPos = enemyPos;
            float checkRadius = activator.Controller.radius;

            int checkCount = 0;
            float distance = 1f;

            while (Physics.CheckSphere(teleportPos, checkRadius, skill.CollidableLayer) && checkCount <= MAX_COLLISION_CHECK)
            {
                ++checkCount;
                distance += CHECK_DISTANCE_OFFSET;
                teleportPos = enemyPos + playerToEnemyDir * distance;
            }

            teleportPos.y = 0f;
            return teleportPos;
        }

        #region GIZMOS
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(activator.transform.position, skill.DetectRadius);
        }
        #endregion
    }
}
