
namespace EverScord.Armor
{
    public class HelmetAugment : ArmorAugment
    {
        public float BasicAttackBonus   { get; private set; }
        public float SkillAttackBonus   { get; private set; }
        public float BasicHealBonus     { get; private set; }
        public float SkillHealBonus     { get; private set; }
        public float AllroundHealBonus  { get; private set; }

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
