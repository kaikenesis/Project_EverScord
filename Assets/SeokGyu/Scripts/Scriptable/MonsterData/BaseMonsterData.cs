using UnityEngine;

namespace EverScord
{
    public class BaseMonsterData : ScriptableObject
    {
        [SerializeField] private float maxHealth;
        [SerializeField] private float moveSpeed;
        [SerializeField] private float power;

        public float MaxHealth
        {
            get { return maxHealth; }
            protected set { maxHealth = value; }
        }

        public float MoveSpeed
        {
            get { return moveSpeed; }
            protected set { moveSpeed = value; }
        }

        public float Power
        {
            get { return power; }
            protected set { power = value; }
        }
    }
}
