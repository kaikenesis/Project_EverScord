using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ActionNode/BossPattern04")]
public class BossPatternNode04 : BAttackPatternNode
{
    public override void Setup(GameObject gameObject)
    {        
        bossData = GetValue<BossData>("BossData");
        actionNodeImplement = gameObject.AddComponent<BossPattern04_Imp>();
        base.Setup(gameObject);
    }

    public override NodeState Evaluate()
    {
        if (attackable == false && bossData.IsUnderHP(90))
            attackable = true;
        return base.Evaluate();
    }
}
