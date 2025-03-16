using EverScord.Augment;

namespace EverScord.Armor
{
    public interface IShoes : IArmor
    {
        public StatBonus MoveSpeedBonus     { get; }
        public StatBonus CooldownBonus      { get; }
        public StatBonus ReloadSpeedBonus   { get; }

        // BonusType enum 의 순서는 ArmorAugmentSheet.csv 에 나열된 강화 순서와 동일해야 합니다.
        // BonusType order must be identical to ArmorAugmentSheet.csv augment order.
        public enum BonusType
        {
            MoveSpeed,
            Cooldown,
            ReloadSpeed,
        };

        public float GetStat(BonusType bonusType)
        {
            switch (bonusType)
            {
                case BonusType.MoveSpeed:
                    return MoveSpeedBonus.CalculateStat();

                case BonusType.Cooldown:
                    return CooldownBonus.CalculateStat();

                case BonusType.ReloadSpeed:
                    return ReloadSpeedBonus.CalculateStat();

                default:
                    return -1;
            }
        }
    }
}
