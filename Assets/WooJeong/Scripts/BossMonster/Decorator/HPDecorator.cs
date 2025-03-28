using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Decorator/HPDecorator")]
public class HPDecorator : BDecoratorNode
{
    [SerializeField] private int failurePhase;
    [SerializeField] private int usableHP;
    private bool pass = false;

    public override NodeState Evaluate()
    {
        if (bossRPC.Phase == failurePhase)
            return NodeState.FAILURE;

        if (bossRPC.IsUnderHP(usableHP) == true)
            pass = true;

        if (pass == false)
            return NodeState.FAILURE;

        state = children[0].Evaluate();
        return state;
    }
}
