using UnityEngine;

namespace EverScord.BehaviorTree
{
    public class ChasePlayerNode : Node
    {
        private TestEnemy enemy;
        private TestPlayer player;
        private bool isInitialized => player != null && enemy != null;

        public override NodeState Evaluate()
        {
            if (!isInitialized)
            {
                enemy = (TestEnemy)GetData(EnemyBT.ENEMY_DATA);
                player = (TestPlayer)GetData(EnemyBT.PLAYER_DATA);

                if (!isInitialized)
                {
                    Debug.LogWarning("ChasePlayerNode blackboard error");
                    return state = NodeState.FAILURE;
                }
            }

            enemy.transform.position = Vector3.MoveTowards(
                enemy.transform.position,
                player.transform.position,
                (float)(enemy.Speed * Time.deltaTime)
            );

            Debug.Log("Chasing player");
            enemy.transform.LookAt(player.transform.position);
            return state = NodeState.RUNNING;
        }
    }
}
