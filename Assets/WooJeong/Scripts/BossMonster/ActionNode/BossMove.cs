using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ActionNode/BossMove")]
public class BossMove : BehaviorNode
{
    private BossMove_Imp bossMove_Imp;

    public override void Setup(GameObject gameObject)
    {
        bossMove_Imp = gameObject.AddComponent<BossMove_Imp>();
        BossData bossData = GetValue<BossData>("BossData");
        bossMove_Imp.Init(bossData.Speed, bossData.AttackRange);
        base.Setup(gameObject);
    }

    public override NodeState Evaluate()
    {
        if (bossMove_Imp.Evaluate() == NodeState.RUNNING)
            return NodeState.RUNNING;

        return NodeState.SUCCESS;
    }
}
