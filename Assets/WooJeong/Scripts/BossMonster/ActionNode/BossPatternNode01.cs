using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ActionNode/BossPattern01", order = 1)]
public class BossPatternNode01 : BActionNode
{
    protected bool isRunning = false;

    public override void Setup(GameObject gameObject)
    {
        actionNodeImplement = gameObject.AddComponent<BossPattern01_Imp>();
        base.Setup(gameObject);
    }

    public override NodeState Evaluate()
    {
        if (!isRunning)
        {
            int random = Random.Range(0, 10);
            if (random < 5)
            {
                return NodeState.FAILURE;
            }
            isRunning = true;
        }

        state = actionNodeImplement.Evaluate();
        if (state == NodeState.SUCCESS)
            isRunning = false;
        return state;
    }
}
