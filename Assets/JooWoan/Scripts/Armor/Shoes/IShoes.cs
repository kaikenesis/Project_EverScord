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

        public enum BonusType
        {
            MoveSpeed,
            Cooldown,
            ReloadSpeed
        };
    }
}
