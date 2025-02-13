using UnityEngine;

namespace EverScord
{
    public class MB_PatternNode1 : BActionNode
    {
        public override void Setup(GameObject gameObject)
        {
            base.Setup(gameObject);
        }

        public override NodeState Evaluate()
        {
            //if (!isRunning)
            //{
            //    int random = Random.Range(0, 10);
            //    if (random <= 5)
            //    {
            //        return NodeState.FAILURE;
            //    }
            //    isRunning = true;
            //}

            //state = actionNodeImplement.Evaluate();
            //if (state == NodeState.SUCCESS)
            //    isRunning = false;

            return state;
        }
    }
}
