using EverScord.Augment;

namespace EverScord.Armor
{
    public interface IVest
    {
        public float Health                         { get; }
        public float Defense                        { get; }
        public float HealthRegeneration             { get; }

        public StatBonus HealthBonus                { get; }
        public StatBonus DefenseBonus               { get; }
        public StatBonus HealthRegenerationBonus    { get; }
        
        public enum BonusType
        {
            Health,
            Defense,
            HealthRegeneration
        };
    }
}
