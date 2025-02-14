using System.Collections;
using UnityEngine;
using EverScord.Character;

namespace EverScord.Skill
{
    public class BombDeliveryAction : MonoBehaviour, ISkillAction
    {
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

            var effect1 = Instantiate(skill.SmallTeleportEffect, CharacterSkill.SkillRoot);
            effect1.transform.position = activator.transform.position;

            activator.transform.position = GetSafeTeleportPosition(closest);

            var effect2 = Instantiate(skill.TeleportEffect, CharacterSkill.SkillRoot);
            effect2.transform.position = activator.transform.position;
        }

        private Vector3 GetSafeTeleportPosition(Collider enemyCol)
        {
            float enemyRadius = Mathf.Max(enemyCol.bounds.size.x, enemyCol.bounds.size.z) / 2f;
            float playerRadius = activator.Controller.radius;
            float totalOffset = enemyRadius + playerRadius;

            totalOffset *= 1.1f;

            Vector3 teleportDir = (enemyCol.transform.position - activator.transform.position).normalized;
            Vector3 teleportPos = enemyCol.transform.position - totalOffset * teleportDir;
            
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
