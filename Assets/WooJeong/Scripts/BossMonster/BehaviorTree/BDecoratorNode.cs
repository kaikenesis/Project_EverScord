using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BDecoratorNode : BehaviorNode
{
    protected BossRPC bossRPC;

    public override void Setup(GameObject gameObject)
    {
        bossRPC = GetValue<BossRPC>("BossRPC");
        base.Setup(gameObject);
    }
}
