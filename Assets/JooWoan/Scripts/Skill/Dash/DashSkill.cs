using UnityEngine;

namespace EverScord.Skill
{
    [CreateAssetMenu(fileName = "Dash Skill", menuName = "EverScord/Character Skill/Dash Skill")]
    public class DashSkill : CharacterSkill
    {
        [field: SerializeField] public float Duration           { get; private set; }
        [field: SerializeField] public float DashForce          { get; private set; }
        [field: SerializeField] public float SpeedMultiplier    { get; private set; }
        [field: SerializeField] public float TrailRefreshRate   { get; private set; }
        [field: SerializeField] public float TrailFadeRate      { get; private set; }
        [field: SerializeField] public GameObject EffectPrefab  { get; private set; }
        [field: SerializeField] public Material TrailMAT        { get; private set; }
    }
}

