using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SK_121201_AttackState2 : NAttackState
{
    private GameObject player;

    protected override void Setup()
    {
        monsterController = GetComponent<SK_121201_Controller>();
    }
    protected override IEnumerator Attack()
    {

        yield return project = StartCoroutine(ProjectAttackRange(2));

        monsterController.Animator.CrossFade("Attack2", 0.3f);
        float time = monsterController.clipDict["Attack2"];


        monsterController.BoxCollider2.enabled = true;
        yield return new WaitForSeconds(time);
        monsterController.BoxCollider2.enabled = false;

        StartCoroutine(monsterController.CoolDown2());
        attack = null;
        Exit();
    }
}
