using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ActionNode/BossPattern07")]
public class BossPatternNode07 : BAttackPatternNode
{
    public override void Setup(GameObject gameObject)
    {
        bossData = GetValue<BossData>("BossData");
        actionNodeImplement = gameObject.AddComponent<BossPattern07_Imp>();
        base.Setup(gameObject);
    }

    public override NodeState Evaluate()
    {
        if (bossData.Phase == 2)
            return NodeState.FAILURE;

        if (attackable == false && bossData.IsUnderHP(60))
        {
            attackable = true;
            isRunning = true;
            state = actionNodeImplement.Evaluate();
        }
        return base.Evaluate();
    }
}
