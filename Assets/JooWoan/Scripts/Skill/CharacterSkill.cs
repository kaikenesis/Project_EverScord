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
        [field: SerializeField] public float BaseHeal;
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

        public static void SetMarkerColor(GameObject marker, Color32 color)
        {
            ParticleSystem[] particles = marker.GetComponentsInChildren<ParticleSystem>();

            if (particles.Length == 0)
                return;

            for (int i = 0; i < particles.Length; i++)
            {
                ParticleSystem.MainModule settings = particles[i].main;
                settings.startColor = new ParticleSystem.MinMaxGradient(color);
            }
        }

        public static void StopEffectParticles(GameObject effect)
        {
            // Effect will be destroyed due to particle stop action mode
            
            if (effect == null)
                return;

            ParticleSystem[] particles = effect.GetComponentsInChildren<ParticleSystem>();

            for (int i = 0; i < particles.Length; i++)
                particles[i].Stop();
        }
    }

    [System.Serializable]
    public class SkillActionInfo
    {
        [field: SerializeField] public CharacterSkill Skill { get; private set; }
        public SkillAction SkillAction { get; private set; }

        public void Init(CharacterControl activator, int skillIndex, EJob characterJob)
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
