using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NML1_AttackState1 : NAttackState
{
    protected override void Setup()
    {
        monsterController = GetComponent<NML1_Controller>();
    }
    
    protected override IEnumerator Attack()
    {
        monsterController.PlayAnimation("Attack1");
        float time = monsterController.clipDict["Attack1"];

        yield return new WaitForSeconds(time / 3);
        monsterController.BoxCollider1.enabled = true;
        monsterController.PlaySound("NML1_1");
        yield return new WaitForSeconds(time / 3);
        monsterController.BoxCollider1.enabled = false;
        yield return new WaitForSeconds(time / 3);
        StartCoroutine(monsterController.CoolDown1());
        attack = null;
        Exit();
    }
}
