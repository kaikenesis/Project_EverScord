
namespace EverScord.Armor
{
    public class HelmetAugmentBuilder : AugmentBuilder
    {
        public float BasicAttackDamage  { get; private set; }
        public float SkillDamage        { get; private set; }
        public float BasicHealAmount    { get; private set; }
        public float SkillHealAmount    { get; private set; }
        public float AllroundHealAmount { get; private set; }

        public HelmetAugmentBuilder() {}

        public HelmetAugmentBuilder SetName(string name)
        {
            Name = name;
            return this;
        }

        public HelmetAugmentBuilder SetDescription(string description)
        {
            Description = description;
            return this;
        }

        public HelmetAugmentBuilder SetBasicAttackDamage(float basicAttackDamage)
        {
            BasicAttackDamage = basicAttackDamage;
            return this;
        }

        public HelmetAugmentBuilder SetSkillDamage(float skillDamage)
        {
            SkillDamage = skillDamage;
            return this;
        }

        public HelmetAugmentBuilder SetBasicHealAmount(float basicHealAmount)
        {
            BasicHealAmount = basicHealAmount;
            return this;
        }

        public HelmetAugmentBuilder SetSkillHealAmount(float skillHealAmount)
        {
            SkillHealAmount = skillHealAmount;
            return this;
        }

        public HelmetAugmentBuilder SetAllroundHealAmount(float allroundHealAmount)
        {
            AllroundHealAmount = allroundHealAmount;
            return this;
        }

        public HelmetAugment Build()
        {
            return new HelmetAugment(this);
        }
    }
}
