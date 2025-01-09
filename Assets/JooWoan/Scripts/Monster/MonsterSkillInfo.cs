
using System.Collections.Generic;

namespace EverScord.Monster
{
    public struct MonsterSkillInfo
    {
        public string tag;
        public List<int> monsterStages;
        public int monsterType;
        public string name;
        public int skillType;
        public float cooldown;
        public float aoeDiameter;
        public List<float> skillRanges;
    }
}
