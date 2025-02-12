using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ActionNode/BossPattern02")]
public class BossPatternNode02 : BAttackPatternNode
{
    public override void Setup(GameObject gameObject)
    {
        actionNodeImplement = gameObject.AddComponent<BossPattern02_Imp>();
        base.Setup(gameObject);
    }

    public override NodeState Evaluate()
    {
        state = actionNodeImplement.Evaluate();
        return state;
    }
}
