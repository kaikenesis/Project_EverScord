using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EverScord.Character
{
    [CreateAssetMenu(fileName = "_ Sound Info", menuName = "EverScord/Character/Sound Info")]
    public class CharacterSoundInfo : ScriptableObject
    {
        [field: SerializeField] public AssetReference FrontFootstep { get; private set; }
        [field: SerializeField] public AssetReference BackFootstep  { get; private set; }
        [field: SerializeField] public AssetReference Gunshot       { get; private set; }
        [field: SerializeField] public AssetReference Reloading     { get; private set; }
        [field: SerializeField] public AssetReference Reloaded      { get; private set; }
    }
}
