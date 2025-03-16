using EverScord.Augment;

namespace EverScord.Armor
{
    public class Shoes : IShoes
    {
        public StatBonus MoveSpeedBonus     { get; }
        public StatBonus CooldownBonus      { get; }
        public StatBonus ReloadSpeedBonus   { get; }

        public Shoes()
        {
            MoveSpeedBonus      = StatBonus.GetDefaultBonus();
            CooldownBonus       = StatBonus.GetDefaultBonus();
            ReloadSpeedBonus    = StatBonus.GetDefaultBonus();
        }
    }
}
