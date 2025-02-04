using UnityEngine;
using EverScord.Character;

namespace EverScord.Skill
{
    public abstract class CharacterSkill : ScriptableObject
    {
        [field: SerializeField] public string Name;
        [field: SerializeField] public float BaseDamage;
        [field: SerializeField] public float Cooldown;
        [field: SerializeField] public GameObject SkillPrefab;

        protected static Transform skillRoot;

        public virtual void Init(CharacterControl activator, ref SkillActionInfo skillActionInfo)
        {
            if (skillRoot == null)
                skillRoot = GameObject.FindGameObjectWithTag(ConstStrings.TAG_SKILLROOT).transform;

            ISkillAction skillAction = Instantiate(SkillPrefab, skillRoot).GetComponent<ISkillAction>();
            skillActionInfo.SetSkillAction(skillAction);
        }
    }

    [System.Serializable]
    public class SkillActionInfo
    {
        [field: SerializeField] public CharacterSkill Skill { get; private set; }
        public ISkillAction SkillAction { get; private set; }

        public void SetSkillAction(ISkillAction skillAction)
        {
            SkillAction = skillAction;
        }
    }
}
