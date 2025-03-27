using UnityEngine;

namespace EverScord.UI
{
    public class CooldownUIEffect : MonoBehaviour
    {
        [field: SerializeField] public ParticleSystem Effect;
        [field: SerializeField] public int SkillIndex;
    }
}
