using EverScord.Augment;

namespace EverScord.Armor
{
    public interface IShoes
    {
        public float MoveSpeed              { get; }
        public float Cooldown               { get; }
        public float ReloadSpeed            { get; }

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
    }
}
