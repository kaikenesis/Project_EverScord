using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ActionNode/BossPattern01", order = 1)]
public class BossPatternNode01 : BActionNode
{
    public override void Setup(GameObject gameObject)
    {
        actionNodeImplement = gameObject.AddComponent<BossPattern01_Imp>();
        base.Setup(gameObject);
    }
}
