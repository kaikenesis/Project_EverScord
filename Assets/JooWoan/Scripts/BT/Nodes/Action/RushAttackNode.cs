using UnityEngine;

namespace EverScord.BehaviorTree
{
    public class RushAttackNode : Node
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
                    Debug.LogWarning("RushAttackNode blackboard is not initialized");
                    return state = NodeState.FAILURE;
                }
            }

            if (enemy.Hp > enemy.MaxHp * enemy.BeginRushAttack * 0.01)
                return state = NodeState.FAILURE;

            Debug.Log("Initiate RushAttack");
            return state = NodeState.RUNNING;
        }
    }
}
