using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public abstract class NAttackState : MonoBehaviour, IState
{
    protected NController monsterController;
    protected bool canAttack = true;
    protected bool isEnter = false;

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
        canAttack = false;
        //monsterController.Animator.CrossFade("Wait", 0.25f);
        monsterController.PlayAnimation("Wait");
    }

    protected virtual void Update()
    {
        if (!isEnter)
            return;

        if (PhotonNetwork.IsMasterClient)
        {
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
        else
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, remoteRot, 10 * Time.deltaTime);
        }

    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.rotation);
        }
        else
        {
            remoteRot = (Quaternion)stream.ReceiveNext();
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

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("attack");
    }
}
