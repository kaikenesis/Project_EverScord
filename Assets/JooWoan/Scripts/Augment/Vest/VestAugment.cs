
namespace EverScord.Augment
{
    public class VestAugment : ArmorAugment
    {
        public StatBonus HealthBonus                { get; private set; }
        public StatBonus DefenseBonus               { get; private set; }
        public StatBonus HealthRegenerationBonus    { get; private set; }

        public VestAugment(VestAugmentBuilder builder)
        {
            Name                        = builder.Name;
            Description                 = builder.Description;

            HealthBonus                 = builder.HealthBonus;
            DefenseBonus                = builder.DefenseBonus;
            HealthRegenerationBonus     = builder.HealthRegenerationBonus;
        }
    }
}
