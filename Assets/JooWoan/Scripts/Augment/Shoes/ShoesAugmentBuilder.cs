using EverScord.Armor;

namespace EverScord.Augment
{
    public class ShoesAugmentBuilder : AugmentBuilder
    {
        public StatBonus MoveSpeedBonus     { get; private set; }
        public StatBonus CooldownBonus      { get; private set; }
        public StatBonus ReloadSpeedBonus   { get; private set; }

        public ShoesAugmentBuilder()
        {
            ResetBonus();
        }

        public void ResetBonus()
        {
            MoveSpeedBonus      = StatBonus.GetDefaultBonus();
            CooldownBonus       = StatBonus.GetDefaultBonus(); 
            ReloadSpeedBonus    = StatBonus.GetDefaultBonus();
        }

        public ShoesAugmentBuilder SetName(string name)
        {
            Name = name;
            return this;
        }

        public ShoesAugmentBuilder SetDescription(string description)
        {
            Description = description;
            return this;
        }

        public ShoesAugmentBuilder SetBonus(IShoes.BonusType bonusType, float additive, float multiplicative)
        {
            switch (bonusType)
            {
                case IShoes.BonusType.MoveSpeed:
                    MoveSpeedBonus = StatBonus.CreateBonus(additive, multiplicative);
                    break;

                case IShoes.BonusType.Cooldown:
                    CooldownBonus = StatBonus.CreateBonus(additive, multiplicative);
                    break;

                case IShoes.BonusType.ReloadSpeed:
                    ReloadSpeedBonus = StatBonus.CreateBonus(additive, multiplicative);
                    break;

                default:
                    break;
            }

            return this;
        }

        public ShoesAugment Build()
        {
            return new ShoesAugment(this);
        }
    }
}

