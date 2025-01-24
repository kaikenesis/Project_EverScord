using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Decorator/DWaitForSec")]
public class DWaitForSec : BehaviorNode
{
    float curTime = 0;

    public override NodeState Evaluate()
    {
        if (curTime >= 1.5f)
        {
            state = children[0].Evaluate();
            if (state == NodeState.SUCCESS)
            {
                curTime = 0;
                return state;
            }            
        }
        else
            curTime += Time.deltaTime;

        return NodeState.RUNNING;
    }
}
