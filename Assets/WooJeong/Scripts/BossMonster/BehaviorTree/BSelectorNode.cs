using System.Collections.Generic;

[UnityEngine.CreateAssetMenu(menuName = "ScriptableObjects/Composite/SelectorNode")]
public class SelectorNode : BehaviorNode
{
    public override NodeState Evaluate()
    {
        for (int i = 0; i < children.Count; i++)
        {
            switch (children[i].Evaluate())
            {
                case NodeState.FAILURE:
                    continue;

                case NodeState.SUCCESS:
                    state = NodeState.SUCCESS;
                    return state;

                case NodeState.RUNNING:
                    state = NodeState.RUNNING;
                    return state;

                default:
                    continue;
            }
        }

        state = NodeState.FAILURE;
        return state;
    }
}
