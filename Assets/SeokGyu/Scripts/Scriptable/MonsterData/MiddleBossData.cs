using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

namespace EverScord
{
    [SerializeField]
    [CreateAssetMenu(menuName = "EverScord/Monster/MiddleBoss", fileName = "newMiddleBossData")]
    public class MiddleBossData : BaseMonsterData
    {
        [SerializeField] private float[] phaseHP;
        [SerializeField] private float cooldown;
        
        [Space(10f)]
        [Header("LagerPattern")]
        [SerializeField] private int laserCount = 4;
        [SerializeField] private float laserMaxDistance = 50.0f;
        [SerializeField] private float laserDistOffset = 0.8f;
        [SerializeField] private float laserRotSpeed = 80.0f;
        [SerializeField] private float laserCastTime = 2.0f;
        [SerializeField] private float laserActivateTime = 4.0f;
        [SerializeField] private float laserFinishTime = 1.5f;

        public int CurrentPhase()
        {
            float per = HealthPercent();
            int phase = 0;

            for (int i = 0; i < phaseHP.Length; i++)
            {
                if (per <= phaseHP[i])
                    phase++;
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

        #region LaserPattern
        public int LaserCount
        {
            get { return laserCount; }

        }
        public float LaserMaxDistance
        {
            get { return laserMaxDistance; }
            private set { laserMaxDistance = value; }
        }

        public float LaserDistOffset
        {
            get { return laserDistOffset; }
            private set { laserDistOffset = value; }
        }

        public float LaserRotSpeed
        {
            get { return laserRotSpeed; }
            private set { laserRotSpeed = value; }
        }

        public float LaserPatternPlayTime
        {
            get { return laserCastTime + laserActivateTime + laserFinishTime; }
        }

        public float LaserCastTime
        {
            get { return laserCastTime; }
            private set { laserCastTime = value; }
        }

        public float LaserActivateTime
        {
            get { return laserActivateTime; }
            private set { laserActivateTime = value; }
        }

        public float LaserFinishTime
        {
            get { return laserFinishTime; }
            private set { laserFinishTime = value; }
        }
        #endregion // LaserPattern

    }
}
