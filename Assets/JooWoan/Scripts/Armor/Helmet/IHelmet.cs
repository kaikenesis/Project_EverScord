using EverScord.Augment;

namespace EverScord.Armor
{
    public interface IHelmet
    {
        // Dealer
        public float BasicAttackDamage      { get; }
        public float SkillDamage            { get; }

        // Healer
        public float BasicHealAmount        { get; }
        public float SkillHealAmount        { get; }
        public float AllroundHealAmount     { get; }

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
    }
}
