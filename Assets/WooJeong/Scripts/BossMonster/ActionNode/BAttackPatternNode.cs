using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BAttackPatternNode : BActionNode
{
    protected bool isRunning = false;

    public override NodeState Evaluate()
    {
        if (!isRunning)
        {
            int random = Random.Range(1, 10);
            if (random <= 8)
            {
                return NodeState.FAILURE;
            }
            isRunning = true;
        }

        state = actionNodeImplement.Evaluate();
        if (state == NodeState.SUCCESS)
            isRunning = false;

        return state;
    }
}
