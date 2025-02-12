using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ActionNode/BossPattern13")]
public class BossPatternNode13 : BAttackPatternNode
{
    public override void Setup(GameObject gameObject)
    {
        bossData = GetValue<BossData>("BossData");
        actionNodeImplement = gameObject.AddComponent<BossPattern13_Imp>();
        base.Setup(gameObject);
    }

    public override NodeState Evaluate()
    {
        if (bossData.Phase == 2)
            attackable = true;
        return base.Evaluate();
    }
}
