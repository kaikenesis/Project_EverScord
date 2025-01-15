using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SK_112206_AttackState1 : MonoBehaviour, IState
{
    private SK_112206_Controller monsterController;
    private bool canAttack = true;
   
    void Awake()
    {
        monsterController = GetComponent<SK_112206_Controller>();
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
        if(monsterController.IsLookPlayer())
        { 
            canAttack = true;
            StartCoroutine(Attack1()); 
        }
    }

    IEnumerator Attack1()
    {
        //StartCoroutine(ProjectAttackRange());
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

        monsterController.Animator.CrossFade("Attack1", 0.25f);
        float time = monsterController.clipDict["Attack1"];

        yield return new WaitForSeconds(time / 3);
        monsterController.BoxCollider.enabled = true;
        yield return new WaitForSeconds(time / 3);
        monsterController.BoxCollider.enabled = false;
        yield return new WaitForSeconds(time / 3);
        StartCoroutine(monsterController.CoolDown1());
        Exit();
    }

    IEnumerator ProjectAttackRange()
    {
        monsterController.Projector.size = new Vector3(monsterController.AttackRangeX,
                                                        monsterController.AttackRangeY,
                                                        monsterController.AttackRangeZ);
        monsterController.Projector.pivot = new Vector3(0, 0, monsterController.AttackRangeZ / 2);
        monsterController.Projector.enabled = true;
        yield return new WaitForSeconds(1f);
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
