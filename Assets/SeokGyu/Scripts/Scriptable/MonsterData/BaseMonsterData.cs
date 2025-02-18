using UnityEngine;

namespace EverScord
{
    public class BaseMonsterData : ScriptableObject
    {
        [SerializeField] private float maxHealth;
        [SerializeField] private float curHealth;
        [SerializeField] private float moveSpeed;
        [SerializeField] private float power;

        public void Init()
        {
            curHealth = maxHealth;
        }

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

        public float CurHealth
        {
            get { return curHealth; }
            protected set { curHealth = value; }
        }

        public void IncreaseHealth(float value)
        {
            curHealth += value;

            if (curHealth > maxHealth)
                curHealth = maxHealth;
        }

        public void DecreaseHealth(float value)
        {
            curHealth -= value;

            if (curHealth < 0.0f)
                curHealth = 0.0f;
        }

        public float HealthPercent()
        {
            return curHealth / maxHealth;
        }
    }
}
