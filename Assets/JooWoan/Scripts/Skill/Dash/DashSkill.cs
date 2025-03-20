using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EverScord.Skill
{
    [CreateAssetMenu(fileName = "Dash Skill", menuName = "EverScord/Character Skill/Dash Skill")]
    public class DashSkill : CharacterSkill
    {
        [field: SerializeField] public float Duration                               { get; private set; }
        [field: SerializeField] public float DashForce                              { get; private set; }
        [field: SerializeField] public float SpeedMultiplier                        { get; private set; }
        [field: SerializeField] public float TrailRefreshRate                       { get; private set; }
        [field: SerializeField] public float TrailFadeRate                          { get; private set; }
        [field: SerializeField] public GameObject DashTornado                       { get; private set; }
        [field: SerializeField] public GameObject DashSpark                         { get; private set; }
        [field: SerializeField] public Material TrailMAT                            { get; private set; }
        [field: SerializeField] public AssetReference DashSfx1                      { get; private set; }
    }
}

