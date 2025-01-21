using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Decorator/PhaseChecker2")]
public class PhaseChecker2 : BDecoratorNode
{
    public override NodeState Evaluate()
    {
        if (bossData.Phase != 2)
        {
            return NodeState.FAILURE;
        }
     
        state = children[0].Evaluate();
        return state;
    }
}
