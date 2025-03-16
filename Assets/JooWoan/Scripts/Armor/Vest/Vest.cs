using EverScord.Augment;

namespace EverScord.Armor
{
    public class Vest : IVest
    {
        public StatBonus HealthBonus                { get; private set; }
        public StatBonus DefenseBonus               { get; private set; }
        public StatBonus HealthRegenerationBonus    { get; private set; }

        public Vest()
        {
            HealthBonus             = StatBonus.GetDefaultBonus();
            DefenseBonus            = StatBonus.GetDefaultBonus();
            HealthRegenerationBonus = StatBonus.GetDefaultBonus();
        }
    }
}
