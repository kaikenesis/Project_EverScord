using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BDecoratorNode : BehaviorNode
{
    protected BossData bossData;

    public override void Setup(GameObject gameObject)
    {
        bossData = GetValue<BossData>("BossData");
        base.Setup(gameObject);
    }
}
