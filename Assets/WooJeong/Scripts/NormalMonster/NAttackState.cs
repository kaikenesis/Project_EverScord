using EverScord.Character;
using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public abstract class NAttackState : MonoBehaviour, IState
{
    protected NController monsterController;
    protected bool isAttacking = false;
    protected float attackDamage;
    protected Coroutine attack;
    protected Coroutine project;
    protected Coroutine updating;
    protected Quaternion remoteRot;

    protected abstract void Setup();

    void Awake()
    {
        Setup();
    }

    public void Enter()
    {
        isAttacking = false;
        monsterController.PlayAnimation("Wait");
        updating = StartCoroutine(Updating());
    }

    protected virtual IEnumerator Updating()
    {
        while(true)
        {
            if (monsterController.isStun)
            {
                ExitToStun();
                yield break;
            }

            if (monsterController.isDead)
            {
                ExitToDeath();
                yield break;
            }

            if (!isAttacking)
            {
                if (monsterController.CalcDistance() > monsterController.monsterData.StopDistance)
                {
                    isAttacking = false;
                    ExitToRun();
                }

                monsterController.LookPlayer();
                if (monsterController.IsLookPlayer(monsterController.monsterData.StopDistance))
                {
                    isAttacking = true;
                    attack = StartCoroutine(Attack());
                }
            }

            yield return new WaitForSeconds(Time.deltaTime);
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
        StopCoroutine(updating);
        monsterController.WaitState();
    }

    protected void ExitToRun()
    {
        StopCoroutine(updating);
        monsterController.RunState();
    }

    protected void ExitToStun()
    {
        StopCoroutine(updating);
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
        StopCoroutine(updating);
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
