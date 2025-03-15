using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using EverScord.Character;

using Object = UnityEngine.Object;

namespace EverScord.Skill
{
    public abstract class CharacterSkill : ScriptableObject
    {
        [field: SerializeField] public string OffensiveTag      { get; private set; }
        [field: SerializeField] public string SupportTag        { get; private set; }
        [field: SerializeField] public GameObject SkillPrefab   { get; private set; }

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

        public static IEnumerator RegenerateHP(CharacterControl activator, CharacterControl[] targets, float duration, float amount)
        {
            Dictionary<CharacterControl, float> healTimerDict = new();

            for (int i = 0; i < targets.Length; i++)
                healTimerDict[targets[i]] = 0f;

            for (float t = 0f; t <= duration; t += Time.deltaTime)
            {
                for (int i = 0; i < targets.Length; i++)
                {
                    healTimerDict[targets[i]] += Time.deltaTime;

                    if (healTimerDict[targets[i]] >= 1f)
                    {
                        healTimerDict[targets[i]] = 0f;
                        targets[i].IncreaseHP(activator, amount, true);
                    }
                }

                yield return null;
            }
        }
    }

    [System.Serializable]
    public class SkillActionInfo
    {
        [field: SerializeField] public CharacterSkill Skill { get; private set; }
        public SkillAction SkillAction { get; private set; }

        public void Init(CharacterControl activator, int skillIndex, PlayerData.EJob characterJob)
        {
            if (Skill == null)
            {
                Debug.LogWarning($"Skill Scriptable Object is not initialized : from {activator.gameObject.name}");
                return;
            }

            SkillAction skillAction = Object.Instantiate(Skill.SkillPrefab, CharacterSkill.SkillRoot).GetComponent<SkillAction>();

            SkillAction = skillAction;
            SkillAction.Init(activator, Skill, characterJob, skillIndex);
        }
    }
}
