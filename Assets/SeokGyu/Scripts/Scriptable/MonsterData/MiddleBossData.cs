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
        
        [Space(10f)]
        [Header("LagerPattern")]
        [SerializeField] private float lagerMaxDistance = 50.0f;
        [SerializeField] private float lagerDistOffset = 0.8f;
        [SerializeField] private float lagerRotSpeed = 80.0f;
        [SerializeField] private float lagerCastTime = 2.0f;
        [SerializeField] private float lagerActivateTime = 4.0f;
        [SerializeField] private float lagerFinishTime = 1.5f;

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

        #region LagerPattern
        public float LagerMaxDistance
        {
            get { return lagerMaxDistance; }
            private set { lagerMaxDistance = value; }
        }

        public float LagerDistOffset
        {
            get { return lagerDistOffset; }
            private set { lagerDistOffset = value; }
        }

        public float LagerRotSpeed
        {
            get { return lagerRotSpeed; }
            private set { lagerRotSpeed = value; }
        }

        public float LagerPatternPlayTime
        {
            get { return lagerCastTime + lagerActivateTime + lagerFinishTime; }
        }

        public float LagerCastTime
        {
            get { return lagerCastTime; }
            private set { lagerCastTime = value; }
        }

        public float LagerActivateTime
        {
            get { return lagerActivateTime; }
            private set { lagerActivateTime = value; }
        }

        public float LagerFinishTime
        {
            get { return lagerFinishTime; }
            private set { lagerFinishTime = value; }
        }
        #endregion // LagerPattern

    }
}
