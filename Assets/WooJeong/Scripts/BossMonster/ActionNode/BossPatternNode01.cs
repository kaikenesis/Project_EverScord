using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ActionNode/BossPattern01", order = 1)]
public class BossPatternNode01 : BActionNode
{
    public bool isRunning = false;
    public int successChance = 50;

    public override void Setup(GameObject gameObject)
    {
        actionNodeImplement = gameObject.GetComponent<BossPattern01_Imp>();
        if(actionNodeImplement == null)
            gameObject.AddComponent<BossPattern01_Imp>();
        base.Setup(gameObject);
    }

    public override NodeState Evaluate()
    {
        if (!isRunning)
        {
            int random = Random.Range(0, 100);
            if(random < successChance)
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
