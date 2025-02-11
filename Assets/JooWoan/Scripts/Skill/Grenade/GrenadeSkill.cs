using EverScord.Character;
using UnityEngine;

namespace EverScord.Skill
{
    [CreateAssetMenu(fileName = "Grenade Skill", menuName = "EverScord/Character Skill/Grenade Skill")]
    public class GrenadeSkill : CharacterSkill
    {
        [field: SerializeField] public float ExplosionRadius { get; private set; }
    }
}
