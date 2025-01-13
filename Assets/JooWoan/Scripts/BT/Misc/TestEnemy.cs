using UnityEngine;
using EverScord.BehaviorTree;

namespace EverScord
{
    public class TestEnemy : MonoBehaviour, IBlackboard
    {
        public delegate void OnEnemyHurt(float percentage);
        public OnEnemyHurt onEnemyHurt;

        [field: SerializeField] public float Hp { get; protected set; }
        [field: SerializeField] public float MaxHp { get; protected set; }
        [field: SerializeField] public float Speed { get; protected set; }
        [field: SerializeField] public float AttackRange { get; protected set; }
        [field: SerializeField, Range(0, 100)] public float BeginRushAttack { get; protected set; }
        [field: SerializeField, Range(0, 100)] public float BeginLaserAttack { get; protected set; }
        [field: SerializeField, Range(0, 100)] public float BeginEarthShatterAttack { get; protected set; }

        void Awake()
        {
            Hp = MaxHp;
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
                ReduceHp();
        }

        public void ReduceHp()
        {
            Hp -= 10f;

            if (onEnemyHurt != null)
                onEnemyHurt(Hp / MaxHp);
        }
    }
}
