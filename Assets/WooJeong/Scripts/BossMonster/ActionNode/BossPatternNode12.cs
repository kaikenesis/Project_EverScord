using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ActionNode/BossPattern12")]
public class BossPatternNode12 : BAttackPatternNode
{
    public override void Setup(GameObject gameObject)
    {
        bossData = GetValue<BossData>("BossData");
        actionNodeImplement = gameObject.AddComponent<BossPattern12_Imp>();
        base.Setup(gameObject);
    }

    public override NodeState Evaluate()
    {
        if (attackable == false && bossData.Phase == 2)
        {
            attackable = true;
            isRunning = true;
            state = actionNodeImplement.Evaluate();
        }
        return base.Evaluate();
    }
}
