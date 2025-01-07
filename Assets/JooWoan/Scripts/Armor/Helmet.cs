
namespace EverScord.Armor
{
    public class Helmet : IHelmet
    {
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

        public Helmet(float basicAttackDamage, float skillDamage, float basicHealAmount, float skillHealAmount, float allroundHealAmount)
        {
            this.basicAttackDamage   = basicAttackDamage;
            this.skillDamage         = skillDamage;
            this.basicHealAmount     = basicHealAmount;
            this.skillHealAmount     = skillHealAmount;
            this.allroundHealAmount  = allroundHealAmount;

            BasicAttackBonus    = new StatBonus(1f, 1f);
            SkillAttackBonus    = new StatBonus(1f, 1f);
            BasicHealBonus      = new StatBonus(1f, 1f);
            SkillHealBonus      = new StatBonus(1f, 1f);
            AllroundHealBonus   = new StatBonus(1f, 1f);
        }
    }
}
