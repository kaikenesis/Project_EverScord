
namespace EverScord.Armor
{
    public class HelmetAugmentBuilder : AugmentBuilder
    {
        public float BasicAttackBonus   { get; private set; }
        public float SkillAttackBonus   { get; private set; }
        public float BasicHealBonus     { get; private set; }
        public float SkillHealBonus     { get; private set; }
        public float AllroundHealBonus  { get; private set; }

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

        public HelmetAugmentBuilder SetBasicAttackBonus(float basicAttackBonus)
        {
            BasicAttackBonus = basicAttackBonus;
            return this;
        }

        public HelmetAugmentBuilder SetSkillAttackBonus(float skillAttackBonus)
        {
            SkillAttackBonus = skillAttackBonus;
            return this;
        }

        public HelmetAugmentBuilder SetBasicHealBonus(float basicHealBonus)
        {
            BasicHealBonus = basicHealBonus;
            return this;
        }

        public HelmetAugmentBuilder SetSkillHealBonus(float skillHealBonus)
        {
            SkillHealBonus = skillHealBonus;
            return this;
        }

        public HelmetAugmentBuilder SetAllroundHealBonus(float allroundHealBonus)
        {
            AllroundHealBonus = allroundHealBonus;
            return this;
        }

        public HelmetAugment Build()
        {
            return new HelmetAugment(this);
        }
    }
}
