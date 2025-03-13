using System.Collections.Generic;

namespace EverScord
{
    public struct MonsterSkillInfo
    {
        public string tag;
        public string name;
        public float cooldown;
        public float skillRange;
        public List<float> skillSizes;
        public float skillDamage;
        public float maxHpBasedDamage; // skill_hpbase
        public float skillDotDamage;
        // public string skillTrigger;
    }
}
