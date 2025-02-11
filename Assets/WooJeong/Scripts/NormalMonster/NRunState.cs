
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
        monsterController.SetNearestPlayer();
        isEnter = true;
        monsterController.Animator.CrossFade("Run", 0.25f);
    }

    protected virtual void Update()
    {
        if (!isEnter)
            return;

        if (monsterController.isStun)
        {
            isEnter = false;
            ExitToStun();
            return;
        }

        if (monsterController.isDead)
        {
            isEnter = false;
            ExitToDeath();
            return;
        }

        if (monsterController.CalcDistance() < monsterController.monsterData.AttackRangeZ1)
        {
            Exit();
            return;
        }

        Vector3 moveVector = (monsterController.player.transform.position - transform.position).normalized;
        monsterController.LookPlayer();
        transform.Translate(monsterController.monsterData.MoveSpeed * Time.deltaTime * moveVector, Space.World);
    }

    protected virtual IEnumerator RandomAttack()
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
                    monsterController.LastAttack = 1;
                    ExitToAttack1();
                    yield break;
                }
            case 2:
                {
                    monsterController.LastAttack = 2;
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
    }

    public void Exit()
    {
        StartCoroutine(RandomAttack());
    }

    protected void ExitToWait()
    {
        isEnter = false;
        monsterController.WaitState();
    }

    protected void ExitToAttack1()
    {
        isEnter = false;
        monsterController.AttackState1();
    }

    protected void ExitToAttack2()
    {
        isEnter = false;
        monsterController.AttackState2();
    }

    protected void ExitToStun()
    {
        isEnter = false;
        monsterController.StunState();
    }

    protected void ExitToDeath()
    {
        isEnter = false;
        monsterController.DeathState();
    }
}
