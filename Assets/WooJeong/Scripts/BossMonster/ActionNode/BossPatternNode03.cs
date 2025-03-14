using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ActionNode/BossPattern03", order = 3)]
public class BossPatternNode03 : BActionNode
{
    public override void Setup(GameObject gameObject)
    {
        GameObject p3 = new GameObject();
        p3.name = "Pattern03";
        actionNodeImplement = p3.AddComponent<BossPattern03_Imp>();
        base.Setup(gameObject);
    }
}
