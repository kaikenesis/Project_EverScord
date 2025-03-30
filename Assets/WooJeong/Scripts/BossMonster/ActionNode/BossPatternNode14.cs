using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ActionNode/BossPattern14", order = 14)]
public class BossPatternNode14 : BAttackPatternNode
{
    public override void Setup(GameObject gameObject)
    {
        actionNodeImplement = gameObject.AddComponent<BossPattern14_Imp>();
        base.Setup(gameObject);
    }
}
