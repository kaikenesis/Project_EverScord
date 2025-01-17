using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ActionNode/BossPattern1")]
public class BossPatternNode2 : BehaviorNode
{
    private BossPattern2_Imp bossPattern2_Imp;

    public override void Setup(GameObject gameObject)
    {
        BossData bossData = GetValue<BossData>("BossData");
        bossPattern2_Imp = gameObject.AddComponent<BossPattern2_Imp>();
        base.Setup(gameObject);
    }

    public override NodeState Evaluate()
    {
        Debug.Log("p1");
        state = NodeState.SUCCESS;
        return state;
    }
}
