using System.Collections.Generic;

[UnityEngine.CreateAssetMenu(menuName = "ScriptableObjects/Composite/SequenceNode")]
public class BSequenceNode : BehaviorNode
{
    public override NodeState Evaluate()
    {
        bool anyChildIsRunning = false;

        for (int i = 0; i < children.Count; i++)
        {
            switch (children[i].Evaluate())
            {
                case NodeState.FAILURE:
                    state = NodeState.FAILURE;
                    return state;

                case NodeState.SUCCESS:
                    continue;

                case NodeState.RUNNING:
                    anyChildIsRunning = true;
                    continue;

                default:
                    continue;
            }
        }

        state = anyChildIsRunning ? NodeState.RUNNING : NodeState.SUCCESS;
        return state;
    }
}
