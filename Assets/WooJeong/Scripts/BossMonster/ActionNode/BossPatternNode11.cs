using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ActionNode/BossPattern11")]
public class BossPatternNode11 : BAttackPatternNode
{
    public override void Setup(GameObject gameObject)
    {
        bossData = GetValue<BossData>("BossData");
        actionNodeImplement = gameObject.AddComponent<BossPattern11_Imp>();
        base.Setup(gameObject);
    }

    public override NodeState Evaluate()
    {
        if (bossData.Phase == 2)
            attackable = true;
        return base.Evaluate();
    }
}
