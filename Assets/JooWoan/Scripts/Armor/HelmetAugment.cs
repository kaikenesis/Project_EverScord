
namespace EverScord.Armor
{
    public class HelmetAugment : ArmorAugment
    {
        public float BasicAttackDamage  { get; private set; }
        public float SkillDamage        { get; private set; }
        public float BasicHealAmount    { get; private set; }
        public float SkillHealAmount    { get; private set; }
        public float AllroundHealAmount { get; private set; }

        public HelmetAugment(HelmetAugmentBuilder builder)
        {
            Name                = builder.Name;
            Description         = builder.Description;

            BasicAttackDamage   = builder.BasicAttackDamage;
            SkillDamage         = builder.SkillDamage;
            BasicHealAmount     = builder.BasicHealAmount;
            SkillHealAmount     = builder.SkillHealAmount;
            AllroundHealAmount  = builder.AllroundHealAmount;
        }
    }
}
