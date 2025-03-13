using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ActionNode/BossPattern02", order = 2)]
public class BossPatternNode02 : BActionNode
{
    public override void Setup(GameObject gameObject)
    {
        actionNodeImplement = gameObject.AddComponent<BossPattern02_Imp>();
        base.Setup(gameObject);
    }
}
