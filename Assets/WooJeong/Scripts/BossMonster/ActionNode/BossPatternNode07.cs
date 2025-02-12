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
        if (bossData.IsUnderHP(60))
            attackable = true;
        return base.Evaluate();
    }
}
