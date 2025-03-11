using System.Collections.Generic;

namespace EverScord
{
    public struct CharacterSkillInfo
    {
        public string tag;
        public string name;
        public List<int> skillTypes;
        public float cooldown;
        public bool isOffensive;

        public float skillCoefficient;
        public float skillDamage;
        public float skillDotDamage;
        public float skillShield;
        //public int hitType;

        public float skillRange;
        public float skillSize;
        //public string triggerCondition;
    }
}
