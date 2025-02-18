using UnityEngine.AddressableAssets;
using UnityEngine;

namespace EverScord.Skill
{
    [CreateAssetMenu(fileName = "Grenade Skill", menuName = "EverScord/Character Skill/Grenade Skill")]
    public class GrenadeSkill : ThrowSkill
    {
        [field: SerializeField] public AssetReferenceGameObject DamageEffectReference   { get; private set; }
        [field: SerializeField] public AssetReferenceGameObject HealEffectReference     { get; private set; }
        [field: SerializeField] public Color32 StampMarkerColor                         { get; private set; }
        [field: SerializeField] public float ExplosionRadius                            { get; private set; }
    }
}
