
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

        public float BasicAttackBonus
        {
            get { return decoratedHelmet.BasicAttackBonus + augment.BasicAttackBonus; }
        }

        public float SkillAttackBonus
        {
            get { return decoratedHelmet.SkillAttackBonus + augment.SkillAttackBonus; }
        }

        public float BasicHealBonus
        {
            get { return decoratedHelmet.BasicHealBonus + augment.BasicHealBonus; }
        }

        public float SkillHealBonus
        {
            get { return decoratedHelmet.SkillHealBonus + augment.SkillHealBonus; }
        }

        public float AllroundHealBonus
        {
            get { return decoratedHelmet.AllroundHealBonus + augment.AllroundHealBonus; }
        }
    }
}
