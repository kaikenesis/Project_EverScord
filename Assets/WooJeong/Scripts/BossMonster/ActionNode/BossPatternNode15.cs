using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ActionNode/BossPattern15")]
public class BossPatternNode15 : BAttackPatternNode
{
    private bool isUsed = false;

    public override void Setup(GameObject gameObject)
    {
        bossData = GetValue<BossData>("BossData");
        actionNodeImplement = gameObject.AddComponent<BossPattern15_Imp>();
        base.Setup(gameObject);
    }

    public override NodeState Evaluate()
    {
        if (!isUsed) 
            return NodeState.FAILURE;

        if (attackable == false && bossData.Phase == 2 && bossData.IsUnderHP(20))
            attackable = true;

        if (!attackable)
            return NodeState.FAILURE;

        if (!isRunning)
        {
            int random = Random.Range(1, 8);
            if (random <= 1)
            {
                return NodeState.FAILURE;
            }
            isRunning = true;
        }

        state = actionNodeImplement.Evaluate();
        if (state == NodeState.SUCCESS)
        {
            isUsed = true;
            isRunning = false;
        }

        return state;
    }
}
