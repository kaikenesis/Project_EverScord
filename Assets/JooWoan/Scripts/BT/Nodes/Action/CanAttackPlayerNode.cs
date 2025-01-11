using UnityEngine;

namespace EverScord.BehaviorTree
{
    public class CanAttackPlayerNode : Node
    {
        private TestEnemy enemy;
        private TestPlayer player;

        public override NodeState Evaluate()
        {
            if (!isInitialized)
            {
                enemy = (TestEnemy)GetData(EnemyBT.ENEMY_DATA);
                player = (TestPlayer)GetData(EnemyBT.PLAYER_DATA);

                if (!isInitialized)
                {
                    Debug.LogWarning("CanAttackPlayerNode blackboard error");
                    return state = NodeState.FAILURE;
                }
            }

            if (Vector3.Distance(enemy.transform.position, player.transform.position) > enemy.AttackRange)
            {
                Debug.Log("Player is too far.");
                return state = NodeState.FAILURE;
            }

            Debug.Log("Can attack Player!");
            return state = NodeState.SUCCESS;
        }

        public bool isInitialized
        {
            get
            {
                if (enemy == null)
                    return false;

                if (player == null)
                    return false;

                return true;
            }
        }
    }
}
