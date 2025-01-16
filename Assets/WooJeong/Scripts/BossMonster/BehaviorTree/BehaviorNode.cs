using System.Collections.Generic;
using UnityEngine;

public class BehaviorNode : ScriptableObject
{
    public enum NodeState
    {
        RUNNING,
        SUCCESS,
        FAILURE
    }

    protected BehaviorNode parent = null;
    [SerializeField] protected List<BehaviorNode> children = new List<BehaviorNode>();
    protected NodeState state = NodeState.FAILURE;

    public void PrintChildren()
    {
        Debug.Log(this.name);
        int count = children.Count;
        if (children == null)
            return;
        for (int i = 0; i < count; i++)
        {
            children[i].PrintChildren();
        }
    }

    public virtual NodeState Evaluate()
    {
        return state;
    }
}
