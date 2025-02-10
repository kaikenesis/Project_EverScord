using UnityEngine;
using EverScord.Character;
using Photon.Pun;

namespace EverScord.Skill
{
    public abstract class CharacterSkill : ScriptableObject
    {
        [field: SerializeField] public string Name;
        [field: SerializeField] public float BaseDamage;
        [field: SerializeField] public float Cooldown;
        [field: SerializeField] public GameObject SkillPrefab;
    }

    [System.Serializable]
    public class SkillActionInfo
    {
        [field: SerializeField] public CharacterSkill Skill { get; private set; }
        public ISkillAction SkillAction { get; private set; }

        private static Transform skillRoot;

        public void Init(CharacterControl activator, int skillIndex)
        {
            if (skillRoot == null)
                skillRoot = GameObject.FindGameObjectWithTag(ConstStrings.TAG_SKILLROOT).transform;

            if (Skill == null)
            {
                Debug.LogWarning($"Skill Scriptable Object is not initialized : from {activator.gameObject.name}");
                return;
            }

            ISkillAction skillAction = Object.Instantiate(Skill.SkillPrefab, skillRoot).GetComponent<ISkillAction>();

            SkillAction = skillAction;
            SkillAction.Init(activator, Skill, skillIndex);
        }
    }
}
