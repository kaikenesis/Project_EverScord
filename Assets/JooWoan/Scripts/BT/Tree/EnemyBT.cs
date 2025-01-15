using System.Collections.Generic;
using UnityEngine;

namespace EverScord.BehaviorTree
{
    public class EnemyBT : ActTree
    {
        public const string ENEMY_DATA = "EnemyData";
        public const string PLAYER_DATA = "PlayerData";

        [SerializeField] private TestEnemy enemy;

        protected override Node SetupTree()
        {
            Node root = new SelectorNode(new List<Node>
            {
                new SequenceNode(new List<Node>
                {
                    new CanAttackPlayerNode(),
                    new SelectorNode(new List<Node>
                    {
                        new EarthShatterAttackNode(),
                        new LaserAttackNode(),
                        new RushAttackNode()
                    })
                }),
                new ChasePlayerNode()
            });

            root.CreateBlackboard();
            root.SetData(ENEMY_DATA, enemy);
            root.SetData(PLAYER_DATA, GameObject.FindGameObjectWithTag("Player").GetComponent<TestPlayer>());

            return root;
        }
    }
}
