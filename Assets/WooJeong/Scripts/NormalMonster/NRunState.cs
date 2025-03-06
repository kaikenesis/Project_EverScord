
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class NRunState : MonoBehaviour, IState
{
    protected NController monsterController;
    protected PhotonView photonView;
    protected Vector3 remotePos;
    protected Quaternion remoteRot;
    protected NavMeshAgent navMeshAgent;
    protected Coroutine updating;

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
        monsterController.PlayAnimation("Run");
        updating = StartCoroutine(Updating());
    }

    protected virtual IEnumerator Updating()
    {
        while (true)
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

            monsterController.SetNearestPlayer();
            if (monsterController.player != null)
                navMeshAgent.destination = monsterController.player.transform.position;

            if (monsterController.CalcDistance() < navMeshAgent.stoppingDistance)
            {
                Exit();
                yield break;
            }

            yield return new WaitForSeconds(Time.deltaTime);
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
        StopUpdating();
        StartCoroutine(RandomAttack());
    }

    protected void ExitToWait()
    {
        StopUpdating();
        monsterController.WaitState();
    }

    protected void ExitToAttack1()
    {
        StopUpdating();
        monsterController.AttackState1();
    }

    protected void ExitToAttack2()
    {
        StopUpdating();
        monsterController.AttackState2();
    }

    protected void ExitToStun()
    {
        StopUpdating();
        navMeshAgent.destination = transform.position;
        monsterController.StunState();
    }

    protected void ExitToDeath()
    {
        StopUpdating();
        navMeshAgent.destination = transform.position;
        monsterController.DeathState();
    }

    protected void StopUpdating()
    {
        if (updating != null)
            StopCoroutine(updating);
        updating = null;
    }
}
