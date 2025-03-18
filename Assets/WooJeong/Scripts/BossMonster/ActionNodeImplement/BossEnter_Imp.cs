using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnter_Imp : ActionNodeImplement
{
    private bool Entered = false;
    public override NodeState Evaluate()
    {
        Debug.Log("Enter");
        if (Entered)
            return NodeState.SUCCESS;

        if (isEnd)
        {
            Entered = true;
            isEnd = false;
            action = null;
            return NodeState.SUCCESS;
        }

        action ??= StartCoroutine(Act());

        return NodeState.RUNNING;
    }

    protected override IEnumerator Act()
    {
        Debug.Log("Enter start");
        bossRPC.PlayAnimation("Landing", 0);
        yield return new WaitForSeconds(bossRPC.clipDict["Landing"] / 4);
        bossRPC.PlayJumpEffect();
        yield return new WaitForSeconds(bossRPC.clipDict["Landing"] / 4 * 3);

        bossRPC.PlayAnimation("Roar");
        yield return new WaitForSeconds(bossRPC.clipDict["Roar"]);
        isEnd = true;
        Debug.Log("Enter end");
    }
}
