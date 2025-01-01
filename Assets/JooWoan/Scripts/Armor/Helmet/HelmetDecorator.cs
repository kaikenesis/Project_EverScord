using EverScord.Augment;

namespace EverScord.Armor
{
    public class HelmetDecorator : IHelmet
    {
        public IHelmet decoratedHelmet { get; private set; }
        public Helmet originalHelmet   { get; private set; }
        public HelmetAugment augment   { get; private set; }

        public HelmetDecorator(IHelmet decoratedHelmet, HelmetAugment augment)
        {
            this.decoratedHelmet  = decoratedHelmet;
            this.augment          = augment;

            if (originalHelmet == null && decoratedHelmet is Helmet)
                originalHelmet = (Helmet)decoratedHelmet;
            else
                originalHelmet = ((HelmetDecorator)decoratedHelmet).originalHelmet;
        }

        public float BasicAttackDamage
        {
            get { return BasicAttackBonus.CalculateStat(originalHelmet.BasicAttackDamage); }
        }

        public float SkillDamage
        {
            get { return SkillAttackBonus.CalculateStat(originalHelmet.SkillDamage); }
        }

        public float BasicHealAmount
        {
            get { return BasicHealBonus.CalculateStat(originalHelmet.BasicHealAmount); }
        }

        public float SkillHealAmount
        {
            get { return SkillHealBonus.CalculateStat(originalHelmet.SkillHealAmount); }
        }

        public float AllroundHealAmount
        {
            get { return AllroundHealBonus.CalculateStat(originalHelmet.AllroundHealAmount); }
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
