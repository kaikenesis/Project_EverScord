using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ActionNode/BossPattern06", order = 6)]
public class BossPatternNode06 : BAttackPatternNode
{
    public override void Setup(GameObject gameObject)
    {
        actionNodeImplement = gameObject.AddComponent<BossPattern06_Imp>();
        base.Setup(gameObject);
    }
}
