
namespace EverScord.Character
{
    public class BarrierBuff : CharacterBuff
    {
        private float originalDefense;
        private float increaseAmount;

        public BarrierBuff()
        {
            increaseAmount = SkillData.SkillInfoDict["urth_skill_2"].skillShield;
        }

        protected override void Apply()
        {
            originalDefense = target.Stats.Defense;
            target.Stats.SetStat(StatType.DEFENSE, originalDefense + increaseAmount);
        }

        protected override void EndBuff()
        {
            target.Stats.SetStat(StatType.DEFENSE, originalDefense);
            base.EndBuff();
        }
    }
}
