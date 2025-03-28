using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NStunState : MonoBehaviour, IState
{
    protected NController monsterController;

    protected abstract void Setup();

    void Awake()
    {
        Setup();
    }

    public void Enter()
    {
        monsterController.PlayAnimation("Wait");
        if (monsterController.BoxCollider1 != null)
            monsterController.BoxCollider1.enabled = false;
        if (monsterController.BoxCollider2 != null)
            monsterController.BoxCollider2.enabled = false;
        StartCoroutine(Stun());
    }

    private IEnumerator Stun()
    {
        float time = 0f;
        while (true)
        {
            yield return new WaitForSeconds(Time.deltaTime);
            if (monsterController.IsDead)
            {
                ExitToDeath();
                yield break;
            }
            time += Time.deltaTime;
            if (time >= monsterController.StunTime)
            {
                monsterController.IsStun = false;
                Exit();
                yield break;
            } 
        }
    }


    protected virtual IEnumerator RandomAttack()
    {
        if (monsterController.CalcDistance() > monsterController.monsterData.Skill01_RangeZ)
        {
            ExitToRun();
            yield break;
        }

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
                    {
                        monsterController.LastAttack = 2;
                        ExitToAttack2();
                    }
                    else
                    {
                        monsterController.LastAttack = 1;
                        ExitToAttack1();
                    }
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
        monsterController.WaitState();
    }

    protected void ExitToRun()
    {
        monsterController.RunState();
    }

    protected void ExitToAttack1()
    {
        monsterController.AttackState1();
    }

    protected void ExitToAttack2()
    {
        monsterController.AttackState2();
    }
    private void ExitToDeath()
    {
        monsterController.DeathState();
    }
}
