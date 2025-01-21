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
        monsterController.Animator.CrossFade("Dying", 0.3f);
        StartCoroutine(Death());
    }

    private IEnumerator Death()
    {
        yield return new WaitForSeconds(1f);
        Exit();
    }

    public void Exit()
    {
        Destroy(this.gameObject);
    }
}
