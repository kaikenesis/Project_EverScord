using UnityEngine;

namespace EverScord.Skill
{
    [CreateAssetMenu(fileName = "Bomb Delivery Skill", menuName = "EverScord/Character Skill/Bomb Delivery Skill")]
    public class BombDeliverySkill : CharacterSkill
    {
        [field: SerializeField] public GameObject BombPrefab            { get; private set; }
        [field: SerializeField] public GameObject HealEffect            { get; private set; }
        [field: SerializeField] public GameObject TeleportEffect        { get; private set; }
        [field: SerializeField] public GameObject TeleportElectric      { get; private set; }
        [field: SerializeField] public LayerMask CollidableLayer        { get; private set; }
        [field: SerializeField] public float DetectRadius               { get; private set; }
        [field: SerializeField] public float HealAmount                 { get; private set; }
        [field: SerializeField] public float StunDuration               { get; private set; }
    }
}
