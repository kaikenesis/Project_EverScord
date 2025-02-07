using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NWaitState : MonoBehaviour, IState
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
        monsterController.PlayAnimation("Wait");
        Exit();
    }

    private void Update()
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

        monsterController.LookPlayer();
    }

    protected virtual IEnumerator RandomAttack()
    {
        while (true)
        {
            if (monsterController.CalcDistance() > monsterController.monsterData.StopDistance)
            {
                ExitToRun();
                yield break;
            }

            int transition = monsterController.CheckCoolDown();
            switch (transition)
            {
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
