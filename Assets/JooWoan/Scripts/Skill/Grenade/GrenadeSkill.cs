using UnityEngine.AddressableAssets;
using UnityEngine;

namespace EverScord.Skill
{
    [CreateAssetMenu(fileName = "Grenade Skill", menuName = "EverScord/Character/Skill/Grenade Skill")]
    public class GrenadeSkill : ThrowSkill
    {
        [field: SerializeField] public AssetReferenceGameObject DamageEffectReference   { get; private set; }
        [field: SerializeField] public AssetReferenceGameObject HealEffectReference     { get; private set; }
        [field: SerializeField] public AssetReference GrenadeExplodeSfx                 { get; private set; }
        [field: SerializeField] public AssetReference ThrowSfx2                         { get; private set; }
        [field: SerializeField] public Color32 MarkerColor                              { get; private set; }
        [field: SerializeField] public Color32 StampMarkerColor                         { get; private set; }
        [field: SerializeField] public float PoisonedDuration                           { get; private set; }
        [field: SerializeField] public float HealDuration                               { get; private set; }
    }
}
