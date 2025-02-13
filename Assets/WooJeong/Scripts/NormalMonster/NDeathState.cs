using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NDeathState : MonoBehaviour, IState
{
    protected NController monsterController;

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
        yield return new WaitForSeconds(monsterController.clipDict["Dying"]);
        ResourceManager.Instance.ReturnToPool(gameObject, monsterController.GUID);
        Exit();
    }

    public void Exit()
    {
        Destroy(this.gameObject);
    }
}
