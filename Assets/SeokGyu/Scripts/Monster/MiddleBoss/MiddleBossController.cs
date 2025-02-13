using UnityEngine;

namespace EverScord
{
    public class MiddleBossController : MonoBehaviour
    {
        [SerializeField] private MiddleBossData m_Data;
        [SerializeField] private float curHP;

        public float CurHealth
        {
            get { return curHP; }
            private set { curHP = value; }
        }

        private void Awake()
        {
            curHP = m_Data.MaxHealth;
        }

        public void IncreaseHealth(float value)
        {
            curHP += value;

            if (curHP >= m_Data.MaxHealth)
                curHP = m_Data.MaxHealth;
        }

        public void DecreaseHealth(float value)
        {
            curHP -= value;

            if (curHP <= 0.0f)
                curHP = 0.0f;
        }
    }
}
