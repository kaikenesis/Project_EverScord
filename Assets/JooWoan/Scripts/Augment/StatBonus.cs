
namespace EverScord.Augment
{
    public struct StatBonus
    {
        public float additive;
        public float multiplicative;

        public float CalculateStat(float statValue)
        {
            return statValue * (additive + 1f) * multiplicative;
        }

        public static StatBonus GetDefaultBonus()
        {
            StatBonus bonus;
            bonus.additive = 0f;
            bonus.multiplicative = 1f;
            return bonus;
        }

        public static StatBonus CreateBonus(float additive, float multiplicative)
        {
            StatBonus bonus;
            bonus.additive = additive * 0.01f;
            bonus.multiplicative = (multiplicative + 100f) * 0.01f;

            return bonus;
        }

        public static StatBonus MergeBonus(StatBonus bonus1, StatBonus bonus2)
        {
            StatBonus mergedBonus;
            mergedBonus.additive = bonus1.additive + bonus2.additive;
            mergedBonus.multiplicative = bonus1.multiplicative * bonus2.multiplicative;

            return mergedBonus;
        }
    }
}
