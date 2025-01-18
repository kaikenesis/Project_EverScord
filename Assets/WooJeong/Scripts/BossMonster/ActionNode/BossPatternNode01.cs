using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ActionNode/BossPattern1")]
public class BossPatternNode01 : BAttackPatternNode
{
    public override void Setup(GameObject gameObject)
    {
        BossData bossData = GetValue<BossData>("BossData");
        actionNodeImplement = gameObject.AddComponent<BossPattern01_Imp>();
        base.Setup(gameObject);
    }
}
