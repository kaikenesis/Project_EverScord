using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NAttackState : MonoBehaviour, IState
{
    protected NController monsterController;
    protected bool canAttack = true;

    protected abstract void Setup();

    void Awake()
    {
        Setup();
    }

    public void Enter()
    {
        canAttack = false;
        monsterController.Animator.CrossFade("Wait", 0.25f);
    }

    private void Update()
    {
        if (canAttack)
            return;

        if (monsterController.CalcDistance() > monsterController.Distance)
        {
            canAttack = true;
            ExitToRun();
        }

        monsterController.LookPlayer();
        if (monsterController.IsLookPlayer())
        {
            canAttack = true;
            StartCoroutine(Attack());
        }
    }

    protected abstract IEnumerator Attack();

    protected IEnumerator ProjectAttackRange()
    {
        monsterController.Projector.size = new Vector3(monsterController.AttackRangeX,
                                                                monsterController.AttackRangeY,
                                                                monsterController.AttackRangeZ);
        monsterController.Projector.pivot = new Vector3(0, 0, monsterController.AttackRangeZ / 2);
        monsterController.BoxCollider.center = new Vector3(0, 0, monsterController.AttackRangeZ / 2);
        monsterController.BoxCollider.size = new Vector3(monsterController.AttackRangeX,
                                        monsterController.AttackRangeY,
                                        monsterController.AttackRangeZ);
        monsterController.Projector.enabled = true;
        yield return new WaitForSeconds(monsterController.ProjectionTime);
        monsterController.Projector.enabled = false;
    }

    public void Exit()
    {
        if (monsterController.CalcDistance() > monsterController.Distance)
        {
            ExitToRun();
        }
        else
        {
            ExitToWait();
        }
    }

    private void ExitToWait()
    {
        monsterController.WaitState();
    }

    private void ExitToRun()
    {
        monsterController.RunState();
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("attack");
    }
}
