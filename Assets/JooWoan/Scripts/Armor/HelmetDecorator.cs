
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

        public float BasicAttackDamage
        {
            get { return decoratedHelmet.BasicAttackDamage + augment.BasicAttackDamage; }
        }

        public float SkillDamage
        {
            get { return decoratedHelmet.BasicAttackDamage + augment.SkillDamage; }
        }

        public float BasicHealAmount
        {
            get { return decoratedHelmet.BasicHealAmount + augment.BasicHealAmount; }
        }

        public float SkillHealAmount
        {
            get { return decoratedHelmet.SkillHealAmount + augment.SkillHealAmount; }
        }

        public float AllroundHealAmount
        {
            get { return decoratedHelmet.AllroundHealAmount; }
        }
    }
}
