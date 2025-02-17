using UnityEngine;

namespace EverScord.Skill
{
    public class ThrowSkill : CharacterSkill
    {
        protected const float DEFAULT_THROW_GRAVITY = 45f;

        [field: SerializeField] public GameObject ThrowPoint         { get; private set; }
        [field: SerializeField] public GameObject ThrowingObject     { get; private set; }
        [field: SerializeField] public GameObject DestinationMarker  { get; private set; }
        [field: SerializeField] public LayerMask CollisionLayer      { get; private set; }
        [field: SerializeField] public bool DisplayTrajectory        { get; private set; }

        [field: SerializeField, Range(1f, 100f)]
        public float Gravity { get; private set; } = DEFAULT_THROW_GRAVITY;
    }
}

