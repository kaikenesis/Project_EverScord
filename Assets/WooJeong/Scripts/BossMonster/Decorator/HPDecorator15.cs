using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Decorator/HPDecorator15")]
public class HPDecorator15 : HPDecorator
{
    private bool isUsed = false;
    public override NodeState Evaluate()
    {
        if (isUsed)
            return NodeState.FAILURE;

        return base.Evaluate();
    }
}
