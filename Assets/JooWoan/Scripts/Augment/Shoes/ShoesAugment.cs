
namespace EverScord.Augment
{
    public class ShoesAugment : ArmorAugment
    {
        public StatBonus MoveSpeedBonus     { get; private set; }
        public StatBonus CooldownBonus      { get; private set; }
        public StatBonus ReloadSpeedBonus   { get; private set; }

        public ShoesAugment(ShoesAugmentBuilder builder)
        {
            Name                = builder.Name;
            Description         = builder.Description;

            MoveSpeedBonus      = builder.MoveSpeedBonus;
            CooldownBonus       = builder.CooldownBonus;
            ReloadSpeedBonus    = builder.ReloadSpeedBonus;
        }
    }
}
