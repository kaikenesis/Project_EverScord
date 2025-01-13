using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SK_112206_AttackState1 : MonoBehaviour, IState, ICoolDown
{
    private SK_112206_Controller monsterController;
    private BoxCollider boxCollider;
    private float curCool;
    public float CurCool { get { return curCool; } }
   
    void Awake()
    {
        monsterController = GetComponent<SK_112206_Controller>();
        boxCollider = GetComponent<BoxCollider>();
    }

    public void Enter()
    {
        StartCoroutine(Attack1());
    }

    IEnumerator CoolDown()
    {
        curCool = monsterController.CoolDown1;
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            curCool -= 0.1f;
            if (curCool <= 0)
                yield break;
        }
    }

    IEnumerator Attack1()
    {
        //StartCoroutine(ProjectAttackRange());
        monsterController.Projector.size = new Vector3(monsterController.AttackRangeX, 
                                                        monsterController.AttackRangeY,
                                                        monsterController.AttackRangeZ);
        monsterController.Projector.pivot = new Vector3(0, 0, monsterController.AttackRangeZ / 2);
        boxCollider.center = new Vector3(0, 0, monsterController.AttackRangeZ / 2);
        boxCollider.size = new Vector3(monsterController.AttackRangeX, 
                                        monsterController.AttackRangeY, 
                                        monsterController.AttackRangeZ);
        monsterController.Projector.enabled = true;
        yield return new WaitForSeconds(monsterController.ProjectionTime);
        monsterController.Projector.enabled = false;

        monsterController.Animator.CrossFade("Attack1", 0.25f);
        float time = monsterController.clipDict["Attack1"].length;
        var clips = monsterController.Animator.GetCurrentAnimatorClipInfo(0);
        foreach( var c in clips )
        {
            Debug.Log(c.clip.name);
            Debug.Log(c.clip.length);
        }
        yield return new WaitForSeconds(time / 3);
        boxCollider.enabled = true;
        yield return new WaitForSeconds(time / 3);
        boxCollider.enabled = false;
        yield return new WaitForSeconds(time / 3);
        StartCoroutine(CoolDown());
        Exit();
    }

    IEnumerator ProjectAttackRange()
    {
        monsterController.Projector.size = new Vector3(0.5f, 1, 7.5f);
        monsterController.Projector.pivot = new Vector3(0, 0, 7.5f / 2);
        monsterController.Projector.enabled = true;
        yield return new WaitForSeconds(1f);
        monsterController.Projector.enabled = false;
    }

    public void Exit()
    {
        monsterController.IdleState();
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("attack");
    }
}
