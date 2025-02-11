using UnityEngine;

namespace EverScord.Skill
{
    [CreateAssetMenu(fileName = "Dash Skill", menuName = "EverScord/Character Skill/Dash Skill")]
    public class DashSkill : CharacterSkill
    {
        [field: SerializeField] public float Duration { get; private set; }
    }
}

