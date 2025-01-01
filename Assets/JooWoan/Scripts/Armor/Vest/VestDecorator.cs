using EverScord.Augment;

namespace EverScord.Armor
{
    public class VestDecorator : IVest
    {
        public IVest decoratedVest   { get; private set; }
        public Vest originalVest     { get; private set; }
        public VestAugment augment   { get; private set; }

        public VestDecorator(IVest decoratedVest, VestAugment augment)
        {
            this.decoratedVest  = decoratedVest;
            this.augment        = augment;

            if (originalVest == null && decoratedVest is Vest)
                originalVest = (Vest)decoratedVest;
            else
                originalVest = ((VestDecorator)decoratedVest).originalVest;
        }

        public float Health
        {
            get { return HealthBonus.CalculateStat(originalVest.Health); }
        }

        public float Defense
        {
            get { return DefenseBonus.CalculateStat(originalVest.Defense); }
        }

        public float HealthRegeneration
        {
            get { return HealthRegenerationBonus.CalculateStat(originalVest.HealthRegeneration); }
        }

        public StatBonus HealthBonus
        {
            get { return StatBonus.MergeBonus(decoratedVest.HealthBonus, augment.HealthBonus); }
        }

        public StatBonus DefenseBonus
        {
            get { return StatBonus.MergeBonus(decoratedVest.DefenseBonus, augment.DefenseBonus); }
        }

        public StatBonus HealthRegenerationBonus
        {
            get { return StatBonus.MergeBonus(decoratedVest.HealthRegenerationBonus, augment.HealthRegenerationBonus); }
        }
    }
}

