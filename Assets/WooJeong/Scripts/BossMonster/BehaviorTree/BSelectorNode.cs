using System.Collections.Generic;

[UnityEngine.CreateAssetMenu(menuName = "ScriptableObjects/Composite/SelectorNode")]
public class BSelectorNode : BehaviorNode
{
    public override NodeState Evaluate()
    {
        for (int i = start; i < children.Count; i++)
        {
            switch (children[i].Evaluate())
            {
                case NodeState.FAILURE:
                    continue;

                case NodeState.SUCCESS:
                    state = NodeState.SUCCESS;
                    start = 0;
                    return state;

                case NodeState.RUNNING:
                    state = NodeState.RUNNING;
                    start = i;
                    return state;

                default:
                    continue;
            }
        }

        state = NodeState.FAILURE;
        start = 0;
        return state;
    }
}
