using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EverScord.Skill
{
    [CreateAssetMenu(fileName = "Jump Attack Skill", menuName = "EverScord/Character/Skill/Jump Attack Skill")]
    public class JumpAttackSkill : CharacterSkill
    {
        [field: SerializeField] public float Duration                           { get; private set; }
        [field: SerializeField] public float LandingDuration                    { get; private set; }
        [field: SerializeField] public float SlowDuration                       { get; private set; }
        [field: SerializeField] public float SlowedAmount                       { get; private set; }
        [field: SerializeField] public Color32 StampMarkerColor                 { get; private set; }
        [field: SerializeField] public AssetReferenceGameObject StanceEffect    { get; private set; }
        [field: SerializeField] public AssetReferenceGameObject ExplosionEffect { get; private set; }
        [field: SerializeField] public AssetReferenceGameObject ShockwaveEffect { get; private set; }
        [field: SerializeField] public AssetReferenceGameObject SlashEffect     { get; private set; }
        [field: SerializeField] public AssetReferenceGameObject HealEffect      { get; private set; }
        [field: SerializeField] public AssetReferenceGameObject Marker          { get; private set; }
        [field: SerializeField] public AssetReference StanceSfx                 { get; private set; }
        [field: SerializeField] public AssetReference JumpSfx                   { get; private set; }
        [field: SerializeField] public AssetReference JumpSfx2                  { get; private set; }
        [field: SerializeField] public AssetReference ChargeSfx1                { get; private set; }
        [field: SerializeField] public AssetReference ChargeSfx2                { get; private set; }
        [field: SerializeField] public AssetReference ChargeSfx3                { get; private set; }
        [field: SerializeField] public AssetReference ChargeSfx4                { get; private set; }
        [field: SerializeField] public AssetReference LandSfx                   { get; private set; }
        [field: SerializeField] public AssetReference DealerLandSfx             { get; private set; }
        [field: SerializeField] public AssetReference HealerLandSfx             { get; private set; }
        [field: SerializeField] public AssetReference UltEnd                    { get; private set; }
    }
}