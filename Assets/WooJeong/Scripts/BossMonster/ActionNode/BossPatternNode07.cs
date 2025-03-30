using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ActionNode/BossPattern07", order = 7)]
public class BossPatternNode07 : BAttackPatternNode
{
    public override void Setup(GameObject gameObject)
    {
        actionNodeImplement = gameObject.AddComponent<BossPattern07_Imp>();
        base.Setup(gameObject);
    }
}
