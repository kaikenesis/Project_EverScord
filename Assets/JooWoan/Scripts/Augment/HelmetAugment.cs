using EverScord.Armor;

namespace EverScord.Augment
{
    public class HelmetAugment : ArmorAugment, IHelmet
    {
        public StatBonus BasicAttackBonus   { get; private set; }
        public StatBonus SkillAttackBonus   { get; private set; }
        public StatBonus BasicHealBonus     { get; private set; }
        public StatBonus SkillHealBonus     { get; private set; }
        public StatBonus AllroundHealBonus  { get; private set; }

        public HelmetAugment(HelmetAugmentBuilder builder)
        {
            Name                = builder.Name;
            Description         = builder.Description;

            BasicAttackBonus    = builder.BasicAttackBonus;
            SkillAttackBonus    = builder.SkillAttackBonus;
            BasicHealBonus      = builder.BasicHealBonus;
            SkillHealBonus      = builder.SkillHealBonus;
            AllroundHealBonus   = builder.AllroundHealBonus;
        }
    }
}
