using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SK_112301_AttackState2 : NAttackState
{
    private bool isCharge = false;
    private Vector3 moveVector;

    protected override void Setup()
    {
        monsterController = GetComponent<SK_112301_Controller>();
        var temp = (SK_112301_Controller)monsterController;
        //temp.AttackRangeX2
    }

    protected override void Update()
    {
        if(isCharge)
        {
            transform.position = Vector3.Lerp(transform.position, moveVector * 7.5f, Time.deltaTime * 1.5f);
            Debug.Log(transform.position);
            Debug.Log(moveVector * 7.5f);
        }

        base.Update();
    }

    protected override IEnumerator Attack()
    {
        yield return ProjectAttackRange();
        moveVector = (monsterController.player.transform.position - transform.position).normalized;

        monsterController.Animator.CrossFade("Attack2", 0.25f);
        float time = monsterController.clipDict["Attack2"];

        yield return new WaitForSeconds(time/4);
        monsterController.BoxCollider.enabled = true;
        isCharge = true;

        yield return new WaitForSeconds(time/4*3);
        isCharge = false;
        monsterController.BoxCollider.enabled = false;
        StartCoroutine(monsterController.CoolDown2());
        Exit();
    }
}
