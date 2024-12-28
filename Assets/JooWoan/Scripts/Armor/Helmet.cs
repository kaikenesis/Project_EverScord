
namespace EverScord.Armor
{
    public class Helmet : IHelmet
    {
        public StatBonus BasicAttackBonus   { get; private set; }
        public StatBonus SkillAttackBonus   { get; private set; }
        public StatBonus BasicHealBonus     { get; private set; }
        public StatBonus SkillHealBonus     { get; private set; }
        public StatBonus AllroundHealBonus  { get; private set; }

        private float basicAttackDamage;
        private float skillDamage;
        private float basicHealAmount;
        private float skillHealAmount;
        private float allroundHealAmount;

        public Helmet(HelmetBuilder builder)
        {
            basicAttackDamage   = builder.BasicAttackDamage;
            skillDamage         = builder.SkillDamage;
            basicHealAmount     = builder.BasicHealAmount;
            skillHealAmount     = builder.SkillHealAmount;
            allroundHealAmount  = builder.AllroundHealAmount;
        }

        public float BasicAttackDamage
        {
            get { return BasicAttackBonus.CalculateStat(basicAttackDamage); }
        }

        public float SkillDamage
        {
            get { return SkillAttackBonus.CalculateStat(skillDamage); }
        }

        public float BasicHealAmount
        {
            get { return BasicHealBonus.CalculateStat(basicHealAmount); }
        }

        public float SkillHealAmount
        {
            get { return SkillHealBonus.CalculateStat(skillHealAmount); }
        }

        public float AllroundHealAmount
        {
            get { return AllroundHealBonus.CalculateStat(allroundHealAmount); }
        }
    }
}
