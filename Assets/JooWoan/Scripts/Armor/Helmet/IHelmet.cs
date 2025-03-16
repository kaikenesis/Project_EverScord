using EverScord.Augment;

namespace EverScord.Armor
{
    public interface IHelmet : IArmor
    {
        public StatBonus BasicAttackBonus   { get; }
        public StatBonus SkillAttackBonus   { get; }
        public StatBonus BasicHealBonus     { get; }
        public StatBonus SkillHealBonus     { get; }
        public StatBonus AllroundHealBonus  { get; }

        // BonusType enum 의 순서는 ArmorAugmentSheet.csv 에 나열된 강화 순서와 동일해야 합니다.
        // BonusType order must be identical to ArmorAugmentSheet.csv augment order.
        public enum BonusType
        {
            BasicAttack,
            SkillAttack,
            BasicHeal,
            SkillHeal,
            AllroundHeal,
        };

        public float GetStat(BonusType bonusType)
        {
            switch (bonusType)
            {
                case BonusType.BasicAttack:
                    return BasicAttackBonus.CalculateStat();

                case BonusType.SkillAttack:
                    return SkillAttackBonus.CalculateStat();

                case BonusType.BasicHeal:
                    return BasicHealBonus.CalculateStat();

                case BonusType.SkillHeal:
                    return SkillHealBonus.CalculateStat();

                case BonusType.AllroundHeal:
                    return AllroundHealBonus.CalculateStat();

                default:
                    return -1;
            }
        }
    }
}
