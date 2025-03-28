using System.Collections;
using UnityEngine;

public abstract class NWaitState : MonoBehaviour, IState
{
    protected NController monsterController;
    protected bool isEnter = false;
    protected Coroutine coGameEnd;

    protected abstract void Setup();
    
    void Awake()
    {
        Setup();
    }

    public void Enter()
    {
        isEnter = true;
        monsterController.PlayAnimation("Wait");
        //Exit();
    }

    private void Update()
    {
        if (!isEnter)
            return;

        if (monsterController.IsStun)
        {
            isEnter = false;
            ExitToStun();
            return;
        }

        if (monsterController.IsDead)
        {
            isEnter = false;
            ExitToDeath();
            return;
        }

        if (monsterController.Player == null)
            monsterController.SetNearestPlayer();
        if (monsterController.Player == null && coGameEnd == null)
            coGameEnd = StartCoroutine(monsterController.GameEnd());

        monsterController.LookPlayer();

        if (monsterController.CalcDistance() > monsterController.monsterData.StopDistance)
        {
            ExitToRun();
            return;
        }

        int transition = monsterController.CheckCoolDown();
        switch (transition)
        {
            case 1:
                {
                    monsterController.LastAttack = 1;
                    ExitToAttack1();
                    return;
                }
            case 2:
                {
                    monsterController.LastAttack = 2;
                    ExitToAttack2();
                    return;
                }
            case 3:
                {
                    if (monsterController.LastAttack == 1)
                    {
                        monsterController.LastAttack = 2;
                        ExitToAttack2();
                    }
                    else
                    {
                        monsterController.LastAttack = 1;
                        ExitToAttack1();
                    }
                    return;
                }
        }
    }

    protected virtual IEnumerator RandomAttack()
    {
        while (true)
        {
            

            yield return new WaitForSeconds(0.1f);
        }

    }

    public void Exit()
    {
        StartCoroutine(RandomAttack());
    }

    protected void ExitToRun()
    {
        isEnter = false;
        monsterController.RunState();
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
