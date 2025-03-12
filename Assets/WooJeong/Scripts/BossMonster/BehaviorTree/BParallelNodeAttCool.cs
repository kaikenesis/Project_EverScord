using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Composite/ParallelNodeAttCool")]
public class BParallelNodeAttCool : BehaviorNode
{
    private int completeFirst = 0;
    private int completeCount = 0;

    public override NodeState Evaluate()
    {
        Debug.Log(completeFirst);
        switch (completeFirst)
        {
            case 0:
            {
                for (int i = 0; i < children.Count; i++)
                {
                    NodeState temp = children[i].Evaluate();

                    if (temp == NodeState.SUCCESS)
                    {
                        completeFirst = i + 1;
                        completeCount++;
                    }
                }
                break;
            }
            case 1:
            {
                NodeState temp = children[1].Evaluate();
                if (temp == NodeState.SUCCESS)
                {
                    completeCount++;
                }
                break;
            }
            case 2:
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
        completeFirst = 0;
        return NodeState.SUCCESS;
    }
}
