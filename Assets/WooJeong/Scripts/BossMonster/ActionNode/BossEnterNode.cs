using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ActionNode/BossEnterNode")]
public class BossEnterNode : BActionNode
{
    public override void Setup(GameObject gameObject)
    {
        actionNodeImplement = gameObject.AddComponent<BossEnter_Imp>();
        base.Setup(gameObject);
    }
}
