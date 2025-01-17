using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class BossPattern1_Imp : MonoBehaviour
{
    private Coroutine attack;
    private bool isEnd = false;

    public NodeState Evaluate()
    {
        if(isEnd)
            return NodeState.SUCCESS;

        if (attack != null)
            return NodeState.RUNNING;
        else
            attack = StartCoroutine(Attack());
     
        return NodeState.RUNNING;
    }

    private IEnumerator Attack()
    {
        isEnd = false;
        yield return new WaitForSeconds(5f);
        isEnd = true;
    }
}
