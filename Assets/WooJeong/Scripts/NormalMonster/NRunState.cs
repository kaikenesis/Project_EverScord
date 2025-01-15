
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NRunState : MonoBehaviour, IState
{
    protected NController monsterController;
    protected bool isEnter = false;

    protected abstract void Setup();

    void Awake()
    {
        Setup();
    }

    public void Enter()
    {
        isEnter = true;
        monsterController.Animator.CrossFade("Run", 0.25f);
    }

    public void Update()
    {
        if (!isEnter)
            return;

        if (monsterController.CalcDistance() < monsterController.Distance)
        {
            Exit();
            return;
        }

        Vector3 moveVector = (monsterController.player.transform.position - transform.position).normalized;
        monsterController.LookPlayer();
        transform.Translate(monsterController.MoveSpeed * Time.deltaTime * moveVector, Space.World);
    }

    IEnumerator RandomAttack()
    {
        while (true)
        {
            int transition = monsterController.CheckCoolDown();
            switch (transition)
            {
                case 0:
                    {
                        ExitToWait();
                        yield break;
                    }
                case 1:
                    {
                        ExitToAttack1();
                        yield break;
                    }
                case 2:
                    {
                        ExitToAttack2();
                        yield break;
                    }
                case 3:
                    {
                        if (monsterController.LastAttack == 1)
                            ExitToAttack2();
                        else
                            ExitToAttack1();
                        yield break;
                    }
            }

            yield return new WaitForSeconds(0.1f);
        }

    }

    public void Exit()
    {
        isEnter = false;
        StartCoroutine(RandomAttack());
    }

    private void ExitToWait()
    {
        monsterController.WaitState();
    }

    private void ExitToAttack1()
    {
        monsterController.AttackState1();
    }

    private void ExitToAttack2()
    {
        monsterController.AttackState2();
    }
}
