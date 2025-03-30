using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ActionNode/BossPattern13", order = 13)]
public class BossPatternNode13 : BAttackPatternNode
{
    public override void Setup(GameObject gameObject)
    {
        actionNodeImplement = gameObject.AddComponent<BossPattern13_Imp>();
        base.Setup(gameObject);
    }
}
