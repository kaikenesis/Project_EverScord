using UnityEngine;

namespace EverScord.BehaviorTree
{
    public class EarthShatterAttackNode : Node
    {
        private TestEnemy enemy;
        private bool isInitialized => enemy != null;

        public override NodeState Evaluate()
        {
            if (!isInitialized)
            {
                enemy = (TestEnemy)GetData(EnemyBT.ENEMY_DATA);

                if (!isInitialized)
                {
                    Debug.LogWarning("EarthShatterAttack blackboard is not initialized");
                    return state = NodeState.FAILURE;
                }
            }

            if (enemy.Hp > enemy.MaxHp * enemy.BeginEarthShatterAttack * 0.01)
                return state = NodeState.FAILURE;

            if (!RollDice())
                return state = NodeState.FAILURE;

            Debug.Log("Initiate EarthShatterAttack");
            return state = NodeState.RUNNING;
        }
    }
}
