using UnityEngine;

namespace EverScord.Character
{
    [CreateAssetMenu(fileName = "_ Animation Info", menuName = "EverScord/Character/Animation Info")]
    public class AnimationInfo : ScriptableObject
    {
        [field: SerializeField] public RuntimeAnimatorController AnimController { get; private set; }
        [field: SerializeField] public Avatar CharacterAvatar                   { get; private set; }

        [field: SerializeField] public AnimationClip Idle           { get; private set; }
        [field: SerializeField] public AnimationClip Shoot          { get; private set; }
        [field: SerializeField] public AnimationClip ShootEnd       { get; private set; }
        [field: SerializeField] public AnimationClip ShootStance    { get; private set; }
        [field: SerializeField] public AnimationClip Reload         { get; private set; }
        [field: SerializeField] public AnimationClip Death          { get; private set; }
        [field: SerializeField] public AnimationClip Revive         { get; private set; }
        [field: SerializeField] public AnimationClip CounterStance  { get; private set; }
        [field: SerializeField] public AnimationClip Howl           { get; private set; }
        [field: SerializeField] public AnimationClip Jump           { get; private set; }
        [field: SerializeField] public AnimationClip Land           { get; private set; }
        [field: SerializeField] public AnimationClip ThrowReady     { get; private set; }
        [field: SerializeField] public AnimationClip Throw          { get; private set; }
        [field: SerializeField] public AnimationClip RunForward     { get; private set; }
    }
}
