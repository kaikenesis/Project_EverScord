using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EverScord.Skill
{
    [CreateAssetMenu(fileName = "Bomb Delivery Skill", menuName = "EverScord/Character Skill/Bomb Delivery Skill")]
    public class BombDeliverySkill : CharacterSkill
    {
        [field: SerializeField] public GameObject BombPrefab            { get; private set; }
        [field: SerializeField] public GameObject TeleportElectric      { get; private set; }
        [field: SerializeField] public GameObject ImpactEffect          { get; private set; }
        [field: SerializeField] public GameObject HealCircleEffect      { get; private set; }
        [field: SerializeField] public LayerMask CollidableLayer        { get; private set; }
        [field: SerializeField] public float HealDuration               { get; private set; }
        [field: SerializeField] public float StunDuration               { get; private set; }
        [field: SerializeField] public AssetReference BombSfx1          { get; private set; }
    }
}
