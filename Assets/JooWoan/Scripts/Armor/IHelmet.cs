
namespace EverScord.Armor
{
    public interface IHelmet
    {
        // Dealer
        public float BasicAttackBonus   { get; }
        public float SkillAttackBonus   { get; }

        // Healer
        public float BasicHealBonus     { get; }
        public float SkillHealBonus     { get; }
        public float AllroundHealBonus  { get; }
    }
}
