using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EverScord.Skill
{
    [CreateAssetMenu(fileName = "Jump Attack Skill", menuName = "EverScord/Character Skill/Jump Attack Skill")]
    public class JumpAttackSkill : CharacterSkill
    {
        [field: SerializeField] public float Duration               { get; private set; }
        [field: SerializeField] public GameObject StanceEffect      { get; private set; }
        [field: SerializeField] public GameObject JumpLandEffect    { get; private set; }
        [field: SerializeField] public GameObject Marker            { get; private set; }
    }
}