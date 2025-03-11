using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ActionNode/BossPattern05")]
public class BossPatternNode05 : BActionNode
{
    public override void Setup(GameObject gameObject)
    {
        actionNodeImplement = gameObject.AddComponent<BossPattern05_Imp>();
        base.Setup(gameObject);
    }
}
