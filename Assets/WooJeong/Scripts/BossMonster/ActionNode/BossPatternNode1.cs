using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ActionNode/BossPattern1")]
public class BossPatternNode1 : BehaviorNode
{
    private BossPattern1_Imp bossPattern1_Imp;

    public override void Setup(GameObject gameObject)
    {
        BossData bossData = GetValue<BossData>("BossData");
        bossPattern1_Imp = gameObject.AddComponent<BossPattern1_Imp>();
        base.Setup(gameObject);
    }

    public override NodeState Evaluate()
    {
        Debug.Log("p1");
        state = NodeState.SUCCESS;
        return state;
    }
}
