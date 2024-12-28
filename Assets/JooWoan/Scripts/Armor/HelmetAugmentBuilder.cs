
namespace EverScord.Armor
{
    public class HelmetAugmentBuilder : AugmentBuilder
    {
        public StatBonus BasicAttackBonus   { get; private set; }
        public StatBonus SkillAttackBonus   { get; private set; }
        public StatBonus BasicHealBonus     { get; private set; }
        public StatBonus SkillHealBonus     { get; private set; }
        public StatBonus AllroundHealBonus  { get; private set; }

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

        public HelmetAugmentBuilder SetBonus(IHelmet.BonusType type, float additive, float multiplicative)
        {
            switch (type)
            {
                case IHelmet.BonusType.BasicAttack:
                    BasicAttackBonus.Init(additive, multiplicative);
                    break;

                case IHelmet.BonusType.SkillAttack:
                    BasicAttackBonus.Init(additive, multiplicative);
                    break;

                case IHelmet.BonusType.BasicHeal:
                    BasicHealBonus.Init(additive, multiplicative);
                    break;

                case IHelmet.BonusType.SkillHeal:
                    SkillHealBonus.Init(additive, multiplicative);
                    break;

                case IHelmet.BonusType.AllroundHeal:
                    AllroundHealBonus.Init(additive, multiplicative);
                    break;

                default:
                    break;
            }

            return this;
        }

        public HelmetAugment Build()
        {
            return new HelmetAugment(this);
        }
    }
}
