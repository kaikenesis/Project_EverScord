using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

namespace EverScord
{
    [SerializeField]
    [CreateAssetMenu(menuName = "EverScord/Monster/MiddleBoss", fileName = "newMiddleBossData")]
    public class MiddleBossData : BaseMonsterData
    {
        [SerializeField] private float[] phaseHealth;
        [SerializeField] private float cooldown;

        public int CurrentPhase(float curHealth)
        {
            float per = curHealth / MaxHealth;
            int phase = -1;

            for (int i = 0; i < phaseHealth.Length; i++)
            {
                if (phaseHealth[i] >= per)
                    phase = i + 1;
                else
                    return phase;
            }

            return phase;
        }

        public float Cooldown
        {
            get { return cooldown; }
            private set { cooldown = value; }
        }
    }
}
