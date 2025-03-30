using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ActionNode/BossPattern03", order = 3)]
public class BossPatternNode03 : BAttackPatternNode
{
    public override void Setup(GameObject gameObject)
    {
        GameObject p3 = new GameObject();
        p3.name = "Pattern03";
        p3.transform.parent = gameObject.transform;
        p3.transform.localPosition = Vector3.zero;
        actionNodeImplement = p3.AddComponent<BossPattern03_Imp>();
        base.Setup(gameObject);
    }
}
