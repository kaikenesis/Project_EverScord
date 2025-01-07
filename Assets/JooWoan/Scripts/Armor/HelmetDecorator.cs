
namespace EverScord.Armor
{
    public class HelmetDecorator : IHelmet
    {
        private readonly IHelmet decoratedHelmet;
        private readonly HelmetAugment augment;

        public HelmetDecorator(IHelmet decoratedHelmet, HelmetAugment augment)
        {
            this.decoratedHelmet  = decoratedHelmet;
            this.augment          = augment;
        }

        public StatBonus BasicAttackBonus
        {
            get { return StatBonus.MergeBonus(decoratedHelmet.BasicAttackBonus, augment.BasicAttackBonus); }
        }

        public StatBonus SkillAttackBonus
        {
            get { return StatBonus.MergeBonus(decoratedHelmet.SkillAttackBonus, augment.SkillAttackBonus); }
        }

        public StatBonus BasicHealBonus
        {
            get { return StatBonus.MergeBonus(decoratedHelmet.BasicHealBonus, augment.BasicHealBonus); }
        }

        public StatBonus SkillHealBonus
        {
            get { return StatBonus.MergeBonus(decoratedHelmet.SkillHealBonus, augment.SkillHealBonus); }
        }

        public StatBonus AllroundHealBonus
        {
            get { return StatBonus.MergeBonus(decoratedHelmet.AllroundHealBonus, augment.AllroundHealBonus); }
        }
    }
}
