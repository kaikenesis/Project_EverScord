using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SK_112206_AttackState1 : NAttackState
{
    protected override void Setup()
    {
        monsterController = GetComponent<SK_112206_Controller>();
    }

    protected override IEnumerator Attack()
    {
        yield return ProjectAttackRange();

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
}
