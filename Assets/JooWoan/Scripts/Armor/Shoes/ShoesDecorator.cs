using EverScord.Augment;

namespace EverScord.Armor
{
    public class ShoesDecorator : IShoes
    {
        public IShoes decoratedShoes { get; private set; }
        public Shoes originalShoes   { get; private set; }
        public ShoesAugment augment   { get; private set; }

        public ShoesDecorator(IShoes decoratedShoes, ShoesAugment augment)
        {
            this.decoratedShoes  = decoratedShoes;
            this.augment         = augment;

            if (originalShoes == null && decoratedShoes is Shoes)
                originalShoes = (Shoes)decoratedShoes;
            else
                originalShoes = ((ShoesDecorator)decoratedShoes).originalShoes;
        }

        public StatBonus MoveSpeedBonus
        {
            get { return StatBonus.MergeBonus(decoratedShoes.MoveSpeedBonus, augment.MoveSpeedBonus); }
        }

        public StatBonus CooldownBonus
        {
            get { return StatBonus.MergeBonus(decoratedShoes.CooldownBonus, augment.CooldownBonus); }
        }

        public StatBonus ReloadSpeedBonus
        {
            get { return StatBonus.MergeBonus(decoratedShoes.ReloadSpeedBonus, augment.ReloadSpeedBonus); }
        }
    }
}
