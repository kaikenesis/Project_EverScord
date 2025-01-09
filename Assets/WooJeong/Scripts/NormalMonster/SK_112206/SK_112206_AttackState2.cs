using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SK_112206_AttackState2 : MonoBehaviour, IState, ICoolDown
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
        StartCoroutine(Attack2());
    }

    IEnumerator CoolDown()
    {
        curCool = monsterController.CoolDown2;
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            curCool -= 0.1f;
            if (curCool <= 0)
                yield break;
        }
    }

    IEnumerator Attack2()
    {
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

        monsterController.Animator.CrossFade("Attack2", 0.25f);        
        float time = monsterController.clipDict["Attack2"];
        
        for (int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(time / 4 / 3);
            boxCollider.enabled = true;
            yield return new WaitForSeconds(time / 4 / 3);
            boxCollider.enabled = false;
            yield return new WaitForSeconds(time / 4 / 3);
        }
        yield return new WaitForSeconds(time / 4);
        StartCoroutine(CoolDown());
        Exit();
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
