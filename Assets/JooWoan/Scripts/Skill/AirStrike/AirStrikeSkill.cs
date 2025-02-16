using UnityEngine;

namespace EverScord.Skill
{
    [CreateAssetMenu(fileName = "Air Strike Skill", menuName = "EverScord/Character Skill/Air Strike Skill")]

    public class AirStrikeSkill : CharacterSkill
    {
        [field: SerializeField] public GameObject Flare { get; private set; }
        [field: SerializeField] public GameObject ThrowPoint { get; private set; }
    }
}
