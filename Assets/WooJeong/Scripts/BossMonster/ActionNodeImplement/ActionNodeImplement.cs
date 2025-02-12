using Photon.Pun;
using System.Collections;
using UnityEngine;

public abstract class ActionNodeImplement : MonoBehaviour
{
    protected Coroutine action;
    protected bool isEnd = false;
    protected BossData bossData;
    protected GameObject player;
    protected BossRPC bossRPC;

    protected virtual void Awake()
    {
        bossRPC = GetComponent<BossRPC>();
    }

    public virtual void Setup(BossData bossData)
    {
        this.bossData = bossData;
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
