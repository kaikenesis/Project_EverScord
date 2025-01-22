using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SK_121201_AttackState1 : NAttackState
{
    private GameObject player;

    protected override void Setup()
    {
        monsterController = GetComponent<SK_121201_Controller>();
    }
    protected override IEnumerator Attack()
    {
        yield return project = StartCoroutine(ProjectAttackRange(1));

        monsterController.Animator.CrossFade("Attack1", 0.3f);
        float time = monsterController.clipDict["Attack1"];


        monsterController.BoxCollider1.enabled = true;
        yield return new WaitForSeconds(time);
        monsterController.BoxCollider1.enabled = false;

        StartCoroutine(monsterController.CoolDown1());
        attack = null;
        Exit();
    }
}
