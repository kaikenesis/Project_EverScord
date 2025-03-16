using EverScord.Augment;

namespace EverScord.Armor
{
    public class Helmet : IHelmet
    {
        public StatBonus BasicAttackBonus   { get; private set; }
        public StatBonus SkillAttackBonus   { get; private set; }
        public StatBonus BasicHealBonus     { get; private set; }
        public StatBonus SkillHealBonus     { get; private set; }
        public StatBonus AllroundHealBonus  { get; private set; }

        public Helmet()
        {
            BasicAttackBonus    = StatBonus.GetDefaultBonus();
            SkillAttackBonus    = StatBonus.GetDefaultBonus();
            BasicHealBonus      = StatBonus.GetDefaultBonus();
            SkillHealBonus      = StatBonus.GetDefaultBonus();
            AllroundHealBonus   = StatBonus.GetDefaultBonus();
        }
    }
}
