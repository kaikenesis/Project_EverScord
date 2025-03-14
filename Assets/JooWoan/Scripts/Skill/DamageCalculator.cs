using EverScord.Character;
using EverScord.Weapons;
using UnityEngine;

namespace EverScord.Skill
{
    public static class DamageCalculator
    {
        public static float GetBulletDamage(int viewID)
        {
            CharacterControl character = GameManager.Instance.PlayerDict[viewID];
            float totalDamage = character.Attack;

            // Calculate damage

            return totalDamage;
        }

        public static float GetSkillDamage(CharacterControl character, CharacterSkill skill)
        {
            float totalDamage = skill.BaseDamage;

            // Calculate damage

            return totalDamage;
        }

        public static float GetAttackDamage(float attack, float critChance, float enemyDefense)
        {
            float defenseRate = enemyDefense / (enemyDefense + 100);
            float damage = attack * (1 - defenseRate);

            if(critChance > 0)
            {
                float randFloat = Random.Range(0.0f, 100.0f);
                if (randFloat <= critChance)
                    damage *= 1.2f;
            }

            int randInt = Random.Range(0, 100);
            if (randInt < 5)
                damage *= 3;
            else if (randInt < 30)
                damage *= 2;            

            return damage;
        }
        
        public static float GetSkillDamage(float attack, float skillDamage, float skillAttackRate, float skillDamageIncrease, float enemyDefense)
        {
            float defenseRate = enemyDefense / (enemyDefense + 100);
            float damage = skillDamage + attack * skillAttackRate * defenseRate;
            float totalDamage = damage + damage * (skillDamageIncrease * 0.01f);
            return totalDamage;
        }
    }
}
