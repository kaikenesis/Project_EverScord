using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Decorator/HPDecorator")]
public class HPDecorator : BDecoratorNode
{
    [SerializeField] private int failurePhase;
    [SerializeField] private int usableHP;
    private bool usable = false;

    private void Awake()
    {
        usable = false;
    }
    
    public override NodeState Evaluate()
    {        
        if (bossRPC.Phase == failurePhase)
            return NodeState.FAILURE;

        if (usable == false && bossRPC.IsUnderHP(usableHP) == true)
        {
            Debug.Log($"{children[0]} 사용가능");
            usable = true;
        }

        if (usable == false)
        {
            Debug.Log($"{children[0]} 사용 불가");
            return NodeState.FAILURE;
        }

        state = children[0].Evaluate();
        return state;
    }
}
