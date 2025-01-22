using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public abstract class NAttackState : MonoBehaviour, IState
{
    protected NController monsterController;
    protected bool canAttack = true;
    protected bool isEnter = false;

    protected Coroutine attack;
    protected Coroutine project;

    protected abstract void Setup();

    void Awake()
    {
        Setup();
    }

    public void Enter()
    {
        isEnter = true;
        canAttack = false;
        monsterController.Animator.CrossFade("Wait", 0.25f);
    }

    protected virtual void Update()
    {
        if (!isEnter)
            return;

        if (monsterController.isStun)
        {
            ExitToStun();
            return;
        }

        if (monsterController.isDead)
        {
            ExitToDeath();
            return;
        }

        if (canAttack)
            return;

        if (monsterController.CalcDistance() > monsterController.monsterData.AttackRangeZ1)
        {
            canAttack = true;
            ExitToRun();
        }

        monsterController.LookPlayer();
        if (monsterController.IsLookPlayer(monsterController.monsterData.AttackRangeZ1))
        {
            canAttack = true;
            attack = StartCoroutine(Attack());
        }
    }

    protected abstract IEnumerator Attack();

    protected virtual IEnumerator ProjectAttackRange(int attackNum)
    {
        DecalProjector projector;
        if (attackNum == 1)
            projector = monsterController.Projector1;
        else
            projector = monsterController.Projector2;

        projector.enabled = true;
        yield return new WaitForSeconds(monsterController.monsterData.ProjectionTime);
        projector.enabled = false;
        project = null;
    }

    public virtual void Exit()
    {
        if (monsterController.CalcDistance() > monsterController.monsterData.AttackRangeZ1)
        {
            ExitToRun();
        }
        else
        {
            ExitToWait();
        }
    }

    protected void ExitToWait()
    {
        isEnter = false;
        monsterController.WaitState();
    }

    protected void ExitToRun()
    {
        isEnter = false;
        monsterController.RunState();
    }

    protected void ExitToStun()
    {
        isEnter = false;
        if (attack != null)
            StopCoroutine(attack);
        if (project != null)
            StopCoroutine(project);
        monsterController.Projector1.enabled = false;
        monsterController.BoxCollider1.enabled = false;
        monsterController.StunState();
    }

    protected void ExitToDeath()
    {
        isEnter = false;
        if (attack != null)
            StopCoroutine(attack);
        if (project != null)
            StopCoroutine(project);
        monsterController.Projector1.enabled = false;
        monsterController.BoxCollider1.enabled = false;
        monsterController.DeathState();
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("attack");
    }
}
