using EverScord.Character;

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

        public static float GetSkillDamage(CharacterControl character, float baseDamage)
        {
            float totalDamage = baseDamage;

            // Calculate damage

            return totalDamage;
        }

        public static float GetHealAmount(CharacterControl character, float baseHeal)
        {
            float totalHeal = baseHeal;

            return totalHeal;
        }
    }
}
