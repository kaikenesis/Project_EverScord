using UnityEngine;

namespace EverScord.Character
{
    [CreateAssetMenu(fileName = "_ Animation Info", menuName = "EverScord/Animation Info")]
    public class AnimationInfo : ScriptableObject
    {
        [field: SerializeField] public AnimationClip Idle          { get; private set; }
        [field: SerializeField] public AnimationClip Shoot         { get; private set; }
        [field: SerializeField] public AnimationClip Shoot_End     { get; private set; }
        [field: SerializeField] public AnimationClip Reload        { get; private set; }
    }
}
