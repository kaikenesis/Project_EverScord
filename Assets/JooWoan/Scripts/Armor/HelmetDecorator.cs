
namespace EverScord.Armor
{
    public class HelmetDecorator : IHelmet
    {
        private readonly IHelmet decoratedHelmet;
        private readonly HelmetAugment augment;

        public HelmetDecorator(IHelmet decoratedHelmet, HelmetAugment augment)
        {
            this.decoratedHelmet    = decoratedHelmet;
            this.augment            = augment;
        }

        public StatBonus BasicAttackBonus
        {
            get { return null; }
        }

        public StatBonus SkillAttackBonus
        {
            get { return null; }
        }

        public StatBonus BasicHealBonus
        {
            get { return null; }
        }

        public StatBonus SkillHealBonus
        {
            get { return null; }
        }

        public StatBonus AllroundHealBonus
        {
            get { return null; }
        }
    }
}
