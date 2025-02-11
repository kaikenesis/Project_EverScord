using Photon.Pun;
using System.Collections;
using UnityEngine;

public abstract class ActionNodeImplement : MonoBehaviour
{
    protected Coroutine action;
    protected bool isEnd = false;
    protected BossData bossData;
    protected Animator animator;
    protected PhotonView photonView;
    protected GameObject player;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        animator = GetComponent<Animator>();
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

    protected void PlayAnimation(string animationName)
    {
        if (animator == null)
        {
            photonView = GetComponent<PhotonView>();
            animator = GetComponent<Animator>();
        }
        animator.CrossFade(animationName, 0.3f, -1, 0);
        photonView.RPC("SyncAnimation", RpcTarget.Others, animationName); 
    }

    [PunRPC]
    protected void SyncAnimation(string animationName)
    {
        if (animator == null)
        {
            photonView = GetComponent<PhotonView>();
            animator = GetComponent<Animator>();
        }
        animator.CrossFade(animationName, 0.3f, -1, 0);
    }

    [PunRPC]
    protected void SyncBossProjectile(Vector3 position, float projectileSpeed)
    {
        GameObject go = ResourceManager.Instance.GetFromPool("BossProjectile", position, Quaternion.identity);
        BossProjectile bp = go.GetComponent<BossProjectile>();
        bp.Setup(transform.forward, projectileSpeed);
    }
}
