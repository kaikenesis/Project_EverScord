using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ActionNode/BossPattern08")]
public class BossPatternNode08 : BAttackPatternNode
{
    public override void Setup(GameObject gameObject)
    {
        bossData = GetValue<BossData>("BossData");
        actionNodeImplement = gameObject.AddComponent<BossPattern08_Imp>();
        base.Setup(gameObject);
    }

    public override NodeState Evaluate()
    {
        if (attackable == false && bossData.IsUnderHP(50))
            attackable = true;
        return base.Evaluate();
    }
}
