using UnityEngine;
using Photon.Pun;
using EverScord.Character;
using Object = UnityEngine.Object;
using System;

namespace EverScord.Skill
{
    public abstract class CharacterSkill : ScriptableObject
    {
        [field: SerializeField] public string Name;
        [field: SerializeField] public float BaseDamage;
        [field: SerializeField] public float Cooldown;
        [field: SerializeField] public GameObject SkillPrefab;

        private static Transform skillRoot;
        public static Transform SkillRoot
        {
            get
            {
                if (skillRoot)
                    return skillRoot;
                
                return GameObject.FindGameObjectWithTag(ConstStrings.TAG_SKILLROOT).transform;
            }
        }
    }

    [System.Serializable]
    public class SkillActionInfo
    {
        [field: SerializeField] public CharacterSkill Skill { get; private set; }
        public ISkillAction SkillAction { get; private set; }

        public void Init(CharacterControl activator, int skillIndex, EJob characterJob)
        {
            if (Skill == null)
            {
                Debug.LogWarning($"Skill Scriptable Object is not initialized : from {activator.gameObject.name}");
                return;
            }

            ISkillAction skillAction = Object.Instantiate(Skill.SkillPrefab, CharacterSkill.SkillRoot).GetComponent<ISkillAction>();

            SkillAction = skillAction;
            SkillAction.Init(activator, Skill, characterJob, skillIndex);
        }
    }
}
