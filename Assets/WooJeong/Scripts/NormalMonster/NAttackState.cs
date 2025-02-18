using EverScord.Character;
using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public abstract class NAttackState : MonoBehaviour, IState
{
    protected NController monsterController;
    protected bool isAttacking = false;
    protected bool isEnter = false;
    protected float attackDamage;
    protected Coroutine attack;
    protected Coroutine project;
    protected Quaternion remoteRot;

    protected abstract void Setup();

    void Awake()
    {
        Setup();
    }

    public void Enter()
    {
        isEnter = true;
        isAttacking = false;
        monsterController.PlayAnimation("Wait");
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

        if (isAttacking)
            return;

        if (monsterController.CalcDistance() > monsterController.monsterData.AttackRangeZ1)
        {
            isAttacking = false;
            ExitToRun();
        }

        monsterController.LookPlayer();
        if (monsterController.IsLookPlayer(monsterController.monsterData.AttackRangeZ1))
        {
            isAttacking = true;
            attack = StartCoroutine(Attack());
        }
    }

    protected abstract IEnumerator Attack();

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
        { 
            StopCoroutine(project);
            //if (monsterController.Projector1 != null)
            //    monsterController.ProjectorDisable(monsterController.Projector1);
            //else if (monsterController.Projector2 != null)
            //    monsterController.ProjectorDisable(monsterController.Projector2);
        }

        if(monsterController.Projector1 != null)
            monsterController.ProjectorDisable(1);
        else if(monsterController.Projector2 != null)
            monsterController.ProjectorDisable(2);

        if (monsterController.BoxCollider1 != null)
            monsterController.BoxCollider1.enabled = false;
        else if (monsterController.BoxCollider2 != null)
            monsterController.BoxCollider2.enabled = false;

        monsterController.StunState();
    }

    protected void ExitToDeath()
    {
        isEnter = false;
        if (attack != null)
            StopCoroutine(attack);
        if (project != null)
            StopCoroutine(project);

        if (monsterController.Projector1 != null)
            monsterController.ProjectorDisable(1);
        else if (monsterController.Projector2 != null)
            monsterController.ProjectorDisable(2);

        if (monsterController.BoxCollider1 != null)
            monsterController.BoxCollider1.enabled = false;
        else if (monsterController.BoxCollider2 != null)
            monsterController.BoxCollider2.enabled = false;

        monsterController.DeathState();
    }

}
