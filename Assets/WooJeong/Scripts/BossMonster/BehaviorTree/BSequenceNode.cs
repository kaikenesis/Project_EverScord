using UnityEngine;

[UnityEngine.CreateAssetMenu(menuName = "ScriptableObjects/Composite/SequenceNode")]
public class BSequenceNode : BehaviorNode
{
    public override NodeState Evaluate()
    {
        for (int i = start; i < children.Count; i++)
        {
            switch (children[i].Evaluate())
            {
                case NodeState.FAILURE:
                    state = NodeState.FAILURE;
                    start = 0;
                    return state;

                case NodeState.SUCCESS:
                    continue;

                case NodeState.RUNNING:
                    start = i;
                    state = NodeState.RUNNING;
                    return state;

                default:
                    continue;
            }
        }

        state = NodeState.SUCCESS;
        start = 0;
        return state;
    }
}
