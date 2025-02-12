using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ActionNode/BossPattern10")]
public class BossPatternNode10 : BAttackPatternNode
{
    public override void Setup(GameObject gameObject)
    {
        bossData = GetValue<BossData>("BossData");
        actionNodeImplement = gameObject.AddComponent<BossPattern10_Imp>();
        base.Setup(gameObject);
    }

    public override NodeState Evaluate()
    {
        if (bossData.Phase == 2)
            return NodeState.FAILURE;

        if (attackable == false && bossData.IsUnderHP(30))
            attackable = true;
        return base.Evaluate();
    }
}
