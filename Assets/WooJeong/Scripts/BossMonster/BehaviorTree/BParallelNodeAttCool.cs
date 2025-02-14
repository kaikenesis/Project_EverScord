using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Composite/ParallelNodeAttCool")]
public class BParallelNodeAttCool : BehaviorNode
{
    private int completeFirst = -1;
    private int completeCount = 0;
    public override NodeState Evaluate()
    {
        switch (completeFirst)
        {
            case -1:
            {
                for (int i = start; i < children.Count; i++)
                {
                    NodeState temp = children[i].Evaluate();

                    if (temp == NodeState.SUCCESS)
                    {
                        completeFirst = i;
                        completeCount++;
                    }
                }
                break;
            }
            case 0:
            {
                NodeState temp = children[1].Evaluate();
                if (temp == NodeState.SUCCESS)
                {
                    completeCount++;
                }
                break;
            }
            case 1:
            {
                NodeState temp = children[0].Evaluate();
                if (temp == NodeState.SUCCESS)
                {
                    completeCount++;
                }
                break;
            }
        }

        if (completeCount < children.Count) 
            return NodeState.RUNNING;

        completeCount = 0;
        completeFirst = -1;
        return NodeState.SUCCESS;
    }
}
