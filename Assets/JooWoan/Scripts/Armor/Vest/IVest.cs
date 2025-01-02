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


        // BonusType enum 의 순서는 ArmorAugmentSheet.csv 에 나열된 강화 순서와 동일해야 합니다.
        // BonusType order must be identical to ArmorAugmentSheet.csv augment order.
        public enum BonusType
        {
            Health,
            Defense,
            HealthRegeneration,
        };
    }
}
