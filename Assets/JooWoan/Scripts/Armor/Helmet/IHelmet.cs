using EverScord.Augment;

namespace EverScord.Armor
{
    public interface IHelmet
    {
        // Dealer
        public float BasicAttackDamage      { get; }
        public float SkillDamage            { get; }

        // Healer
        public float BasicHealAmount        { get; }
        public float SkillHealAmount        { get; }
        public float AllroundHealAmount     { get; }

        public StatBonus BasicAttackBonus   { get; }
        public StatBonus SkillAttackBonus   { get; }
        public StatBonus BasicHealBonus     { get; }
        public StatBonus SkillHealBonus     { get; }
        public StatBonus AllroundHealBonus  { get; }

        public enum BonusType
        {
            BasicAttack,
            SkillAttack,
            BasicHeal,
            SkillHeal,
            AllroundHeal
        };
    }
}
