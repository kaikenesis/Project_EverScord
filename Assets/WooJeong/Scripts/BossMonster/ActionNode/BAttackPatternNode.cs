using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BAttackPatternNode : BActionNode
{
    protected BossData bossData;
    protected bool isRunning = false;
    protected bool attackable = false;

    public override NodeState Evaluate()
    {
        if (!attackable)
            return NodeState.FAILURE;

        if (!isRunning)
        {
            int random = Random.Range(1, 8);
            if (random <= 1)
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
