using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ActionNode/BossPattern1")]
public class BossPatternNode01 : BAttackPatternNode
{
    public override void Setup(GameObject gameObject)
    {
        bossData = GetValue<BossData>("BossData");
        actionNodeImplement = gameObject.AddComponent<BossPattern01_Imp>();
        base.Setup(gameObject);
    }

    public override NodeState Evaluate()
    {
        if (!isRunning)
        {
            int random = Random.Range(1, 10);
            Debug.Log(random);
            if (random <= 5)
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
