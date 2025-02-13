using UnityEngine;

namespace EverScord.Skill
{
    [CreateAssetMenu(fileName = "Bomb Delivery Skill", menuName = "EverScord/Character Skill/Bomb Delivery Skill")]
    public class BombDeliverySkill : CharacterSkill
    {
        [field: SerializeField] public GameObject BombPrefab        { get; private set; }
        [field: SerializeField] public GameObject TeleportEffect    { get; private set; }
    }
}
