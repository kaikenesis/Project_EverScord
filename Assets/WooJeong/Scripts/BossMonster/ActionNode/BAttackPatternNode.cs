using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BAttackPatternNode : BActionNode
{
    protected bool isRunning = false;
    protected bool attackable = false;

    public override NodeState Evaluate()
    {
        if (!isRunning)
        {
            int random = Random.Range(1, 9);
            if (random != 1)
            {
                return NodeState.FAILURE;
            }
            Debug.Log($"{this} »ç¿ë");
            isRunning = true;
        }

        state = actionNodeImplement.Evaluate();
        if (state == NodeState.SUCCESS)
            isRunning = false;

        return state;
    }
}
