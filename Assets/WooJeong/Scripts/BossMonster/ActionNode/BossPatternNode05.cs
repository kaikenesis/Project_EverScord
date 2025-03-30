using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ActionNode/BossPattern05", order = 5)]
public class BossPatternNode05 : BAttackPatternNode
{
    public override void Setup(GameObject gameObject)
    {
        actionNodeImplement = gameObject.AddComponent<BossPattern05_Imp>();
        base.Setup(gameObject);
    }
}
