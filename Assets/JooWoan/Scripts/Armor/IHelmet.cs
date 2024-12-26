
namespace EverScord.Armor
{
    public interface IHelmet
    {
        // Dealer
        public float BasicAttackDamage  { get; }
        public float SkillDamage        { get; }

        // Healer
        public float BasicHealAmount    { get; }
        public float SkillHealAmount    { get; }
        public float AllroundHealAmount { get; }
    }
}
