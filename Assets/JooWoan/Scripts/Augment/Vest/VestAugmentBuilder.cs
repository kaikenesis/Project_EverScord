using EverScord.Armor;

namespace EverScord.Augment
{
    public class VestAugmentBuilder : AugmentBuilder
    {
        public StatBonus HealthBonus                { get; private set; }
        public StatBonus DefenseBonus               { get; private set; }
        public StatBonus HealthRegenerationBonus    { get; private set; }

        public VestAugmentBuilder()
        {
            ResetBonus();
        }

        public void ResetBonus()
        {
            HealthBonus                 = StatBonus.GetDefaultBonus();
            DefenseBonus                = StatBonus.GetDefaultBonus(); 
            HealthRegenerationBonus     = StatBonus.GetDefaultBonus();
        }

        public VestAugmentBuilder SetName(string name)
        {
            Name = name;
            return this;
        }

        public VestAugmentBuilder SetDescription(string description)
        {
            Description = description;
            return this;
        }

        public VestAugmentBuilder SetBonus(IVest.BonusType bonusType, float additive, float multiplicative)
        {
            switch (bonusType)
            {
                case IVest.BonusType.Health:
                    HealthBonus = StatBonus.CreateBonus(additive, multiplicative);
                    break;

                case IVest.BonusType.Defense:
                    DefenseBonus = StatBonus.CreateBonus(additive, multiplicative);
                    break;

                case IVest.BonusType.HealthRegeneration:
                    HealthRegenerationBonus = StatBonus.CreateBonus(additive, multiplicative);
                    break;

                default:
                    break;
            }

            return this;
        }

        public VestAugment Build()
        {
            return new VestAugment(this);
        }
    }
}
