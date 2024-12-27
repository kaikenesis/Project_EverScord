
namespace EverScord.Armor
{
    public class Helmet : IHelmet
    {
        public float BasicAttackDamage
        {
            get { return basicAttackDamage * BasicAttackBonus * 0.01f; }
        }

        public float SkillDamage
        {
            get { return skillDamage * SkillAttackBonus * 0.01f; }
        }

        public float BasicHealAmount
        {
            get { return basicHealAmount * BasicHealBonus * 0.01f; }
        }

        public float SkillHealAmount  
        {
            get { return skillHealAmount * SkillHealBonus * 0.01f; }
        }

        public float AllroundHealAmount
        {
            get { return allroundHealAmount * AllroundHealBonus * 0.01f; }
        }

        public float BasicAttackBonus   { get; private set; }
        public float SkillAttackBonus   { get; private set; }
        public float BasicHealBonus     { get; private set; }
        public float SkillHealBonus     { get; private set; }
        public float AllroundHealBonus  { get; private set; }

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

            BasicAttackBonus    = 100.0f;
            SkillAttackBonus    = 100.0f;
            BasicHealBonus      = 100.0f;
            SkillHealBonus      = 100.0f;
            AllroundHealBonus   = 100.0f;
        }
    }
}
