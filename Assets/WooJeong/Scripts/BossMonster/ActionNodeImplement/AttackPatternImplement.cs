using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttackPatternImplement : ActionNodeImplement
{
    protected int coolDown = 10;
    protected int curCool = 0;

    public override NodeState Evaluate()
    {
        if(curCool > 0)
        {
            return NodeState.FAILURE;
        }

        if (isEnd)
        {
            isEnd = false;
            return NodeState.SUCCESS;
        }

        if (action == null)
        {
            StartCoroutine(CoolDown());
            action = StartCoroutine(Action());
        }

        return NodeState.RUNNING;
    }

    protected IEnumerator CoolDown()
    {
        curCool = coolDown;
        while (true)
        {
            yield return new WaitForSeconds(1f);
            curCool--;
            if (curCool <= 0)
                yield break;
        }
    }
}
