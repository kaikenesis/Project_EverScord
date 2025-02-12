using UnityEngine;

namespace EverScord.Skill
{
    [CreateAssetMenu(fileName = "Grenade Skill", menuName = "EverScord/Character Skill/Grenade Skill")]
    public class GrenadeSkill : CharacterSkill
    {
        [field: SerializeField] public GameObject GrenadeStartPoint { get; private set; }
        [field: SerializeField] public GameObject HitMarker         { get; private set; }
        [field: SerializeField] public Rigidbody PoisonBomb         { get; private set; }
        [field: SerializeField] public Rigidbody HealBomb           { get; private set; }
        [field: SerializeField] public float PredictInterval        { get; private set; }
        [field: SerializeField] public float HitMarkerGroundOffset  { get; private set; }
        [field: SerializeField] public float ExplosionRadius        { get; private set; }
        [field: SerializeField] public int MaxPoints                { get; private set; }
    }
}
