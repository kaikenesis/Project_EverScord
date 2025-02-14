using EverScord.Character;
using EverScord.Weapons;

namespace EverScord.Skill
{
    public static class DamageCalculator
    {
        public static float GetBulletDamage(int viewID, Weapon sourceWeapon)
        {
            CharacterControl character = GameManager.Instance.PlayerDict[viewID];
            float totalDamage = sourceWeapon.Damage;

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
