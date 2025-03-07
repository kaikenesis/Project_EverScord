using System.Collections;
using UnityEngine;
using Photon.Pun;

public abstract class NDeathState : MonoBehaviour, IState
{
    protected NController monsterController;
    protected float dissolveDuration = 3f;
    protected abstract void Setup();

    void Awake()
    {
        Setup();
    }

    public void Enter()
    {
        monsterController.PlayAnimation("Dying");
        monsterController.ProjectorDisable(1);
        monsterController.ProjectorDisable(2);
        if (monsterController.BoxCollider1 != null)
            monsterController.BoxCollider1.enabled = false;
        if (monsterController.BoxCollider2 != null)
            monsterController.BoxCollider2.enabled = false;
        StartCoroutine(Death());
    }

    private IEnumerator Death()
    {
        monsterController.PhotonView.RPC(nameof(monsterController.SyncDeathAftermath), RpcTarget.All);
        yield return new WaitForSeconds(monsterController.clipDict["Dying"]);

        monsterController.PhotonView.RPC(nameof(monsterController.SyncDissolve), RpcTarget.All, dissolveDuration);
        yield return new WaitForSeconds(dissolveDuration);

        Exit();
    }

    public void Exit()
    {
        monsterController.Death();
    }
}
