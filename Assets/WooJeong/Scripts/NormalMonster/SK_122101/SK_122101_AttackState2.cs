using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SK_122101_AttackState2 : NAttackState
{
    protected override void Setup()
    {
        monsterController = GetComponent<SK_122101_Controller>();
    }
    
    protected override IEnumerator Attack()
    {
        yield return ProjectAttackRange();

        monsterController.Animator.CrossFade("Attack2", 0.3f);
        float time = monsterController.clipDict["Attack2"];
        monsterController.BoxCollider.enabled = true;
        yield return new WaitForSeconds(time);
        monsterController.BoxCollider.enabled = false;
        StartCoroutine(monsterController.CoolDown2());
        Exit();
    }
}
