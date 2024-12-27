
namespace EverScord.Armor
{
    public class HelmetBuilder
    {
        public float BasicAttackDamage  { get; private set; }
        public float SkillDamage        { get; private set; }
        public float BasicHealAmount    { get; private set; }
        public float SkillHealAmount    { get; private set; }
        public float AllroundHealAmount { get; private set; }

        public HelmetBuilder() {}

        public HelmetBuilder SetBasicAttackDamage(float basicAttackDamage)
        {
            BasicAttackDamage = basicAttackDamage;
            return this;
        }

        public HelmetBuilder SetBasicSkillDamage(float skillDamage)
        {
            SkillDamage = skillDamage;
            return this;
        }

        public HelmetBuilder SetBasicHealAmount(float basicHealAmount)
        {
            BasicHealAmount = basicHealAmount;
            return this;
        }

        public HelmetBuilder SetSkillHealAmount(float skillHealAmount)
        {
            SkillHealAmount = skillHealAmount;
            return this;
        }

        public HelmetBuilder SetAllroundHealAmount(float allroundHealAmount)
        {
            AllroundHealAmount = allroundHealAmount;
            return this;
        }

        public Helmet Build()
        {
            return new Helmet(this);
        }
    }
}
