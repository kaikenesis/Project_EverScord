using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ActionNode/BossMove")]
public class BossMove : BActionNode
{
    public override void Setup(GameObject gameObject)
    {        
        actionNodeImplement = gameObject.AddComponent<BossMove_Imp>();
        BossData bossData = GetValue<BossData>("BossData");
        Animator animator = GetValue<Animator>("Animator");
        actionNodeImplement.Setup(bossData);
        base.Setup(gameObject);
    }
}
