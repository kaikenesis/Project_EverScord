using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ActionNode/BossDeathNode")]
public class BossDeathNode : BActionNode
{
    public override void Setup(GameObject gameObject)
    {
        actionNodeImplement = gameObject.AddComponent<BossDeath_Imp>();
        BossData bossData = GetValue<BossData>("BossData");
        actionNodeImplement.Setup(bossData);
        base.Setup(gameObject);
    }

    public override NodeState Evaluate()
    {        
        state = actionNodeImplement.Evaluate();
        return state;
    }
}
