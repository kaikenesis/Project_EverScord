using UnityEngine;

namespace EverScord.Skill
{
    public class ThrowSkill : CharacterSkill
    {
        [field: SerializeField] public GameObject ThrowPoint         { get; private set; }
        [field: SerializeField] public GameObject DestinationMarker  { get; private set; }
        [field: SerializeField] public LayerMask CollisionLayer      { get; private set; }
        [field: SerializeField] public bool DisplayTrajectory        { get; private set; }

        [field: SerializeField, Range(1f, 100f)]
        public float Gravity { get; private set; }
    }
}

