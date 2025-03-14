using EverScord.Character;
using EverScord.Weapons;

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
    }
}
