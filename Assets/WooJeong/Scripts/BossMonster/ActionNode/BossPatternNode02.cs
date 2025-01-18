using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ActionNode/BossPattern2")]
public class BossPatternNode02 : BAttackPatternNode
{
    public override void Setup(GameObject gameObject)
    {
        BossData bossData = GetValue<BossData>("BossData");
        actionNodeImplement = gameObject.AddComponent<BossPattern02_Imp>();
        base.Setup(gameObject);
    }
}
