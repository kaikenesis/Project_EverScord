using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ActionNode/BoosCoolDownNode")]
public class BoosCoolDownNode : BActionNode
{
    public override void Setup(GameObject gameObject)
    {
        actionNodeImplement = gameObject.AddComponent<BossCoolDown_Imp>();
        base.Setup(gameObject);
    }
    
    public override NodeState Evaluate()
    {
        state = actionNodeImplement.Evaluate();
        return state;
    }
}
