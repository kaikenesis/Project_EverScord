using UnityEngine;

namespace EverScord.Character
{
    [CreateAssetMenu(fileName = "_ Weapon Info", menuName = "EverScord/Character/Weapon Info")]
    public class WeaponInfo : ScriptableObject
    {
        [field: SerializeField] public GameObject WeaponPrefab  { get; private set; }
        [field: SerializeField] public GameObject IconPrefab    { get; private set; }
        [field: SerializeField] public Material TracerMaterial  { get; private set; }
        [field: SerializeField] public Gradient TracerGradient  { get; private set; }
        [field: SerializeField] public float Cooldown           { get; private set; }
        [field: SerializeField] public float ReloadTime         { get; private set; }
        [field: SerializeField] public float WeaponRange        { get; private set; }
        [field: SerializeField] public int MaxAmmo              { get; private set; }
        [field: SerializeField] public float BulletSpeed        { get; private set; }
    }
}