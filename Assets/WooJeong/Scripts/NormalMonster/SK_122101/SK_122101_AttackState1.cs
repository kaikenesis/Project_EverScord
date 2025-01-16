using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SK_122101_AttackState1 : NAttackState
{
    protected override void Setup()
    {
        monsterController = GetComponent<SK_122101_Controller>();
    }
    
    protected override IEnumerator Attack()
    {
        yield return ProjectAttackRange();

        monsterController.Animator.CrossFade("Attack1", 0.25f);
        float time = monsterController.clipDict["Attack1"] + monsterController.clipDict["Attack1_Loop"];
        yield return new WaitForSeconds(time);
        monsterController.BoxCollider.enabled = true;
        time = monsterController.clipDict["Attack1_Attack"];
        yield return new WaitForSeconds(time);
        monsterController.BoxCollider.enabled = false;
        time = monsterController.clipDict["Attack1_End"];
        yield return new WaitForSeconds(time);
        StartCoroutine(monsterController.CoolDown1());
        Exit();
    }
}
