using EverScord.Monster;
using Photon.Pun;
using System.Collections;
using Unity.Burst.CompilerServices;
using UnityEngine;

public abstract class ActionNodeImplement : MonoBehaviour
{
    protected Coroutine action = null;
    protected bool isEnd = false;
    protected BossRPC bossRPC;

    protected virtual void Awake()
    {
        bossRPC = GetComponent<BossRPC>();
    }
    
    public virtual NodeState Evaluate()
    {
        if (isEnd)
        {
            isEnd = false;
            action = null;
            return NodeState.SUCCESS;
        }

        if (action == null)
            action = StartCoroutine(Act());

        return NodeState.RUNNING;
    }

    protected abstract IEnumerator Act();


}
