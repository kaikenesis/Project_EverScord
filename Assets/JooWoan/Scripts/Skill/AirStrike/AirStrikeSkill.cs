using UnityEngine.AddressableAssets;
using UnityEngine;

namespace EverScord.Skill
{
    [CreateAssetMenu(fileName = "Air Strike Skill", menuName = "EverScord/Character Skill/Air Strike Skill")]

    public class AirStrikeSkill : ThrowSkill
    {
        [field: SerializeField] public float ExplosionRadius                                { get; private set; }
        [field: SerializeField] public float ExplosionInterval                              { get; private set; }
        [field: SerializeField] public float ZoneDuration                                   { get; private set; }
        [field: SerializeField] public float ZoneInfluenceInterval                          { get; private set; }
        [field: SerializeField] public int BombCount                                        { get; private set; }
        [field: SerializeField] public float AirCraftSpeed                                  { get; private set; }
        [field: SerializeField] public float AirCraftTravelDistance                         { get; private set; }
        [field: SerializeField] public GameObject AircraftPrefab                            { get; private set; }
        [field: SerializeField] public Color32 AirStrikeMarkerColor                         { get; private set; }
        [field: SerializeField] public AssetReferenceGameObject ExplosionEffectReference    { get; private set; }
        [field: SerializeField] public AssetReferenceGameObject HealExplosionReference     { get; private set; }
        [field: SerializeField] public AssetReferenceGameObject BombEffectReference         { get; private set; }
        [field: SerializeField] public AssetReferenceGameObject FlameEffectReference        { get; private set; }
        [field: SerializeField] public AssetReferenceGameObject HealZoneEffectReference     { get; private set; }
    }
}
