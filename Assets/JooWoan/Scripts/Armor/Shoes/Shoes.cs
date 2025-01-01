using EverScord.Augment;

namespace EverScord.Armor
{
    public class Shoes : IShoes
    {
        public float MoveSpeed              { get; }
        public float Cooldown               { get; }
        public float ReloadSpeed            { get; }

        public StatBonus MoveSpeedBonus     { get; }
        public StatBonus CooldownBonus      { get; }
        public StatBonus ReloadSpeedBonus   { get; }

        public Shoes(float moveSpeed, float cooldown, float reloadSpeed)
        {
            MoveSpeed           = moveSpeed;
            Cooldown            = cooldown;
            ReloadSpeed         = reloadSpeed;

            MoveSpeedBonus      = StatBonus.GetDefaultBonus();
            CooldownBonus       = StatBonus.GetDefaultBonus();
            ReloadSpeedBonus    = StatBonus.GetDefaultBonus();
        }
    }
}
