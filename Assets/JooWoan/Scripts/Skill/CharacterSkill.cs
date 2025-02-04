using EverScord.Character;
using UnityEngine;

namespace EverScord.Skill
{
    public abstract class CharacterSkill : ScriptableObject
    {
        [field: SerializeField] public string Name;
        [field: SerializeField] public float BaseDamage;
        [field: SerializeField] public float Cooldown;
        [field: SerializeField] public GameObject SkillPrefab;

        protected static Transform skillRoot;
        protected ISkillAction skillAction;

        public CharacterSkill()
        {
            if (skillRoot == null)
                skillRoot = GameObject.FindGameObjectWithTag(ConstStrings.TAG_SKILLROOT).transform;

            var prefab = Instantiate(SkillPrefab, skillRoot);
            skillAction = prefab.GetComponent<ISkillAction>();
        }

        public virtual void Activate(CharacterControl activator, EJob ejob)
        {
            skillAction.Activate(ejob);
        }
    }
}
