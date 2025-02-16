using UnityEngine;
using EverScord.Character;

namespace EverScord.Skill
{
    public class GrenadeImpact : MonoBehaviour
    {
        private CharacterControl activator;
        private GrenadeSkill skill;

        void OnDisable()
        {
            if (activator.CharacterJob == EJob.HEALER)
                ApplyHeal();
            else
                ApplyDamage();
        }

        public void Init(CharacterControl activator, GrenadeSkill skill)
        {
            this.activator = activator;
            this.skill = skill;
        }

        private void ApplyHeal()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, skill.ExplosionRadius, GameManager.PlayerLayer);
            float calculatedHeal = skill.BaseHeal;

            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].transform.root == activator.transform)
                    continue;

                CharacterControl player = colliders[i].GetComponent<CharacterControl>();
                player.IncreaseHP(calculatedHeal);
            }
        }

        private void ApplyDamage()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, skill.ExplosionRadius, GameManager.EnemyLayer);
            float calculatedDamage = DamageCalculator.GetSkillDamage(activator, skill);

            for (int i = 0; i < colliders.Length; i++)
            {
                IEnemy enemy = colliders[i].GetComponent<IEnemy>();
                GameManager.Instance.EnemyHitsControl.ApplyDamageToEnemy(calculatedDamage, enemy);
            }
        }
    }
}
