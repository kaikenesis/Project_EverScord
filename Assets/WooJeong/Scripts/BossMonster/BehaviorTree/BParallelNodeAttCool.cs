using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Composite/ParallelNodeAttCool")]
public class BParallelNodeAttCool : BehaviorNode
{    
    private BossData bossData;

    public override void Setup(GameObject gameObject)
    {
        bossData = GetValue<BossData>("BossData");
        base.Setup(gameObject);
    }

    public override NodeState Evaluate()
    {
        for (int i = start; i < children.Count; i++)
        {
            NodeState temp = children[i].Evaluate();

            if (temp == NodeState.SUCCESS)
            {
                start++;
            }

        }
        if (start < children.Count) 
            return NodeState.RUNNING;

        start = 0;
        return NodeState.SUCCESS;
    }
}
