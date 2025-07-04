using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EverScord.Skill
{
    [CreateAssetMenu(fileName = "Counter Skill", menuName = "EverScord/Character Skill/Counter Skill")]
    public class CounterSkill : CharacterSkill
    {

        [field: SerializeField, Range(0.0f, 1.0f)]
        public float LaserLerpRate { get; private set; }
        [field: SerializeField] public float Duration               { get; private set; }
        [field: SerializeField] public float DamageInterval         { get; private set; }
        [field: SerializeField] public GameObject BarrierPrefab     { get; private set; }
        [field: SerializeField] public GameObject LaserPrefab       { get; private set; }
        [field: SerializeField] public AssetReference BuffSfx       { get; private set; }
        [field: SerializeField] public AssetReference LaserStartSfx { get; private set; }
        [field: SerializeField] public AssetReference LaserStartSfx2{ get; private set; }
    }
}
