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

        public float BasicAttackDamage      { get; private set; }
        public float SkillDamage            { get; private set; }
        public float BasicHealAmount        { get; private set; }
        public float SkillHealAmount        { get; private set; }
        public float AllroundHealAmount     { get; private set; }

        public Helmet(float basicAttackDamage, float skillDamage, float basicHealAmount, float skillHealAmount, float allroundHealAmount)
        {
            BasicAttackDamage   = basicAttackDamage;
            SkillDamage         = skillDamage;
            BasicHealAmount     = basicHealAmount;
            SkillHealAmount     = skillHealAmount;
            AllroundHealAmount  = allroundHealAmount;

            BasicAttackBonus    = new StatBonus(1f, 1f);
            SkillAttackBonus    = new StatBonus(1f, 1f);
            BasicHealBonus      = new StatBonus(1f, 1f);
            SkillHealBonus      = new StatBonus(1f, 1f);
            AllroundHealBonus   = new StatBonus(1f, 1f);
        }
    }
}
