using EverScord.Armor;

namespace EverScord.Augment
{
    public class HelmetAugmentBuilder : AugmentBuilder
    {
        public StatBonus BasicAttackBonus   { get; private set; }
        public StatBonus SkillAttackBonus   { get; private set; }
        public StatBonus BasicHealBonus     { get; private set; }
        public StatBonus SkillHealBonus     { get; private set; }
        public StatBonus AllroundHealBonus  { get; private set; }

        public HelmetAugmentBuilder()
        {
            ResetBonus();
        }

        public void ResetBonus()
        {
            BasicAttackBonus  = StatBonus.GetDefaultBonus();
            SkillAttackBonus  = StatBonus.GetDefaultBonus(); 
            BasicHealBonus    = StatBonus.GetDefaultBonus();
            SkillHealBonus    = StatBonus.GetDefaultBonus();
            AllroundHealBonus = StatBonus.GetDefaultBonus();
        }

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

        public HelmetAugmentBuilder SetBonus(IHelmet.BonusType bonusType, float additive, float multiplicative)
        {
            switch (bonusType)
            {
                case IHelmet.BonusType.BasicAttack:
                    BasicAttackBonus = StatBonus.CreateBonus(additive, multiplicative);
                    break;

                case IHelmet.BonusType.SkillAttack:
                    SkillAttackBonus = StatBonus.CreateBonus(additive, multiplicative);
                    break;

                case IHelmet.BonusType.BasicHeal:
                    BasicHealBonus = StatBonus.CreateBonus(additive, multiplicative);
                    break;

                case IHelmet.BonusType.SkillHeal:
                    SkillHealBonus = StatBonus.CreateBonus(additive, multiplicative);
                    break;

                case IHelmet.BonusType.AllroundHeal:
                    AllroundHealBonus = StatBonus.CreateBonus(additive, multiplicative);
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
