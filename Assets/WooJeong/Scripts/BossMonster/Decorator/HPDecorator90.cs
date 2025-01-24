using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Decorator/HPDecorator90")]
public class HPDecorator90 : BDecoratorNode
{
    public override NodeState Evaluate()
    {
        if (bossData.HP > bossData.MaxHP / 100 * 90)
        {
            state = children[0].Evaluate();
            return state;
        }
        else if (state == NodeState.RUNNING)
        {
            state = children[0].Evaluate();
            return state;
        }

        return NodeState.FAILURE;
    }
}
