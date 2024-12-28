
namespace EverScord.Armor
{
    public interface IHelmet
    {
        // Dealer
        public StatBonus BasicAttackBonus   { get; }
        public StatBonus SkillAttackBonus   { get; }

        // Healer
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
