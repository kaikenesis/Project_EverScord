using EverScord.Character;
using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public abstract class NAttackState : MonoBehaviour, IState
{
    protected NController monsterController;
    protected bool isAttacking = false;
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
            if (monsterController.IsStun)
            {
                ExitToStun();
                yield break;
            }

            if (monsterController.IsDead)
            {
                ExitToDeath();
                yield break;
            }

            if (!isAttacking)
            {
                monsterController.SetNearestPlayer();
                if(monsterController.Player == null)
                {
                    ExitToWait();
                    yield break;
                }

                if (monsterController.CalcDistance() > monsterController.monsterData.StopDistance)
                {
                    isAttacking = false;
                    ExitToRun();
                    yield break;
                }

                monsterController.LookPlayer();
                if (monsterController.IsLookPlayer(monsterController.monsterData.StopDistance))
                {
                    isAttacking = true;
                    attack = StartCoroutine(Attack());
                }
            }

            yield return null;
        }
    }

    protected abstract IEnumerator Attack();

    public virtual void Exit()
    {
        if (monsterController.CalcDistance() > monsterController.monsterData.Skill01_RangeZ)
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
        StopUpdating();
        monsterController.WaitState();
    }

    protected void ExitToRun()
    {
        StopUpdating();
        monsterController.RunState();
    }

    protected void ExitToStun()
    {        
        if (attack != null)
            StopCoroutine(attack);
        if (project != null)
            StopCoroutine(project);

        monsterController.StunState();
    }

    protected void ExitToDeath()
    {
        StopUpdating();
        if (attack != null)
            StopCoroutine(attack);
        if (project != null)
            StopCoroutine(project);

        monsterController.DeathState();
    }

    protected void StopUpdating()
    {
        if(updating != null)
            StopCoroutine(updating);
        updating = null;
    }
}
