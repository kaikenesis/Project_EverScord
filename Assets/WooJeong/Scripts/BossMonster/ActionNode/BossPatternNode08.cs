using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ActionNode/BossPattern09", order = 8)]
public class BossPatternNode08 : BActionNode
{
    public override void Setup(GameObject gameObject)
    {
        actionNodeImplement = gameObject.AddComponent<BossPattern08_Imp>();
        base.Setup(gameObject);
    }
}
