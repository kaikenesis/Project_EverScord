using EverScord.Augment;
using EverScord.Character;
using UnityEngine;

namespace EverScord.Skill
{
    public static class DamageCalculator
    {
        public const float BASE_HEAL = 13f;

        public static float GetBulletDamage(int viewID, IEnemy monster = null)
        {
            CharacterControl character = GameManager.Instance.PlayerDict[viewID];

            if (character.CharacterJob == PlayerData.EJob.Dealer && monster != null)
                return GetAttackDamage(character.Stats.Attack, character.Stats.CriticalHitChance, monster.GetDefense());

            return GetBulletHealAmount(character);
        }

        public static float GetBulletHealAmount(CharacterControl character)
        {
            float amount = BASE_HEAL + character.Stats.Attack * 0.3f;
            StatBonus healBonus = character.Stats.BonusDict[StatType.COOLDOWN_DECREASE];

            return healBonus.CalculateStat(amount);
        }

        public static float GetSkillDamage(CharacterControl character, float baseDamage, float coefficient, IEnemy monster = null)
        {
            float attack = character.Stats.Attack;
            float increase = 100f;
            float defense = monster != null ? monster.GetDefense() : 0f;

            float skillDamage = character.Stats.IncreasedSkillDamage(baseDamage);

            return GetSkillDamage(attack, skillDamage, coefficient, increase, defense);
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
        
        public static float GetSkillDamage(float attack, float skillDamage, float skillAttackRate = 1f, float skillDamageIncrease = 100f, float enemyDefense = 0f)
        {
            skillAttackRate = Mathf.Max(1f, skillAttackRate);
            skillDamageIncrease = Mathf.Max(100f, skillDamageIncrease);

            float defenseRate = enemyDefense / (enemyDefense + 100);
            float damage = skillDamage + attack * skillAttackRate * (1 - defenseRate);
            float totalDamage = damage + damage * (skillDamageIncrease * 0.01f);
            Debug.Log($"{defenseRate} / {damage} / {totalDamage}");
            return totalDamage;
        }
    }
}
