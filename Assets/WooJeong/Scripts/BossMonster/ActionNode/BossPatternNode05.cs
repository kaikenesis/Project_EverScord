using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ActionNode/BossPattern05")]
public class BossPatternNode05 : BAttackPatternNode
{
    public override void Setup(GameObject gameObject)
    {
        bossData = GetValue<BossData>("BossData");
        actionNodeImplement = gameObject.AddComponent<BossPattern05_Imp>();
        base.Setup(gameObject);
    }

    public override NodeState Evaluate()
    {
        if (attackable == false && bossData.IsUnderHP(80))
        {
            attackable = true;
            Debug.Log("5 attackable");
        }
        return base.Evaluate();
    }
}
