using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionNodeImplement : MonoBehaviour
{
    protected Coroutine action;
    protected bool isEnd = false;

    public virtual void Setup(BossData bossData)
    {
        return;
    }
    
    public virtual NodeState Evaluate()
    {
        if (isEnd)
        {
            isEnd = false;
            return NodeState.SUCCESS;
        }

        if (action == null)
            action = StartCoroutine(Action());

        return NodeState.RUNNING;
    }

    protected abstract IEnumerator Action();

}
