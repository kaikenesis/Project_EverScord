
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class NRunState : MonoBehaviour, IState
{
    protected NController monsterController;
    protected bool isEnter = false;
    protected PhotonView photonView;
    protected Vector3 remotePos;
    protected Quaternion remoteRot;
    protected NavMeshAgent navMeshAgent;

    protected abstract void Setup();

    void Awake()
    {
        Setup();
        photonView = GetComponent<PhotonView>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.stoppingDistance = monsterController.monsterData.StopDistance;
    }

    public void Enter()
    {
        monsterController.SetNearestPlayer();
        isEnter = true;
        monsterController.PlayAnimation("Run");
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

        navMeshAgent.destination = monsterController.player.transform.position;
        Debug.Log(navMeshAgent.remainingDistance);
        if (monsterController.CalcDistance() < navMeshAgent.stoppingDistance)
        {
            Exit();
            return;
        }
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
