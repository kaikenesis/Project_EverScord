using EverScord.Augment;

namespace EverScord.Armor
{
    public class Vest : IVest
    {
        public float Health                         { get; private set; }
        public float Defense                        { get; private set; }
        public float HealthRegeneration             { get; private set; }

        public StatBonus HealthBonus                { get; private set; }
        public StatBonus DefenseBonus               { get; private set; }
        public StatBonus HealthRegenerationBonus    { get; private set; }

        public Vest(float health, float defense, float healthRegeneration)
        {
            Health                  = health;
            Defense                 = defense;
            HealthRegeneration      = healthRegeneration;

            HealthBonus             = StatBonus.GetDefaultBonus();
            DefenseBonus            = StatBonus.GetDefaultBonus();
            HealthRegenerationBonus = StatBonus.GetDefaultBonus();
        }
    }
}
