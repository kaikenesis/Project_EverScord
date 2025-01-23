using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ActionNode/BossPattern1")]
public class BossPatternNode01 : BAttackPatternNode
{
    BossData bossData;
    
    public override NodeState Evaluate()
    {
        if (bossData.Hp < bossData.MaxHp / 100 * 90)
            return NodeState.FAILURE;

        return base.Evaluate();
    }

    public override void Setup(GameObject gameObject)
    {
        bossData = GetValue<BossData>("BossData");
        actionNodeImplement = gameObject.AddComponent<BossPattern01_Imp>();
        base.Setup(gameObject);
    }
}
