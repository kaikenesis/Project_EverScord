using EverScord.Monster;
using Photon.Pun;
using System.Collections;
using Unity.Burst.CompilerServices;
using UnityEngine;

public abstract class ActionNodeImplement : MonoBehaviour
{
    protected Coroutine action;
    protected bool isEnd = false;
    protected BossData bossData;
    protected GameObject player;
    protected BossRPC bossRPC;
    protected LayerMask playerLayer;

    protected virtual void Awake()
    {
        bossRPC = GetComponent<BossRPC>();
        playerLayer = LayerMask.GetMask("Player");
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
            action = null;
            return NodeState.SUCCESS;
        }

        if (action == null)
            action = StartCoroutine(Act());

        return NodeState.RUNNING;
    }

    protected abstract IEnumerator Act();

    public void LookPlayer()
    {
        if (player != null)
        {
            Vector3 dir = player.transform.position - transform.position;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 20);
            transform.rotation = new(0, transform.rotation.y, 0, transform.rotation.w);
        }
    }

    public bool IsLookPlayer(float distance)
    {
        Vector3 start = new(transform.position.x, transform.position.y + 0.3f, transform.position.z);
        if (Physics.Raycast(start, transform.forward, distance, playerLayer))
        {
            return true;
        }
        Debug.DrawRay(start, transform.forward * distance, Color.red);
        return false;
    }
}
