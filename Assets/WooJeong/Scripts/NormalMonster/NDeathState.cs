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
        StartCoroutine(Death());
    }

    private IEnumerator Death()
    {
        monsterController.PhotonView.RPC(nameof(monsterController.SyncGlitterEffect), RpcTarget.All);
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
