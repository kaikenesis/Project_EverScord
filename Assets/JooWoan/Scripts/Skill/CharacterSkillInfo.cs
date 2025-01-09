
using System.Collections.Generic;

namespace EverScord.Skill
{
    public struct CharacterSkillInfo
    {
        public string tag;
        public string name;
        public List<int> skillTypes;
        public float cooldown;
        public bool isOffensive;
        public string effectType;
        public float effectValue;
        public int hitType;
        public float aoeDiameter;
        public List<float> skillRanges;
        public string triggerCondition;
    }
}
