using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BActionNode : BehaviorNode
{
    protected ActionNodeImplement actionNodeImplement;

    public override NodeState Evaluate()
    {
        state = actionNodeImplement.Evaluate();
        return state;
    }
}
