using UnityEngine;
using EverScord.Character;

using Object = UnityEngine.Object;

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

        public static ParticleSystem[] SetEffectParticles(GameObject effect, bool shouldPlay)
        {
            // Effect will be destroyed due to particle stop action mode
            
            if (effect == null)
                return null;

            ParticleSystem[] particles = effect.GetComponentsInChildren<ParticleSystem>();
            SetEffectParticles(particles, shouldPlay);

            return particles;
        }

        public static void SetEffectParticles(ParticleSystem[] particles, bool shouldPlay)
        {
            if (particles == null)
                return;
            
            for (int i = 0; i < particles.Length; i++)
            {
                if (!shouldPlay)
                    particles[i].Stop();

                else
                {
                    particles[i].Clear();
                    particles[i].Play();
                }
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
