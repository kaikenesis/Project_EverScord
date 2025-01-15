using UnityEngine;

namespace EverScord.BehaviorTree
{
    public class LaserAttackNode : Node
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
                    Debug.LogWarning("LaserAttackNode blackboard is not initialized");
                    return state = NodeState.FAILURE;
                }
            }

            if (enemy.Hp > enemy.MaxHp * enemy.BeginLaserAttack * 0.01)
                return state = NodeState.FAILURE;

            if (!RollDice())
                return state = NodeState.FAILURE;

            Debug.Log("Initiate LaserAttack");
            return state = NodeState.RUNNING;
        }
    }
}
