using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ActionNode/BossMove")]
public class BossMove : BActionNode
{
    public override void Setup(GameObject gameObject)
    {        
        actionNodeImplement = gameObject.AddComponent<BossMove_Imp>();
        base.Setup(gameObject);
    }
}
