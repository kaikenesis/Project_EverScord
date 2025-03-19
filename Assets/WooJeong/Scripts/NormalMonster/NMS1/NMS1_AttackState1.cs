using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NMS1_AttackState1 : NAttackState
{
    protected override void Setup()
    {
        monsterController = GetComponent<NMS1_Controller>();
    }
    
    protected override IEnumerator Attack()
    {
        monsterController.PlayAnimation("Attack1");
        float time = monsterController.clipDict["Attack1"] + monsterController.clipDict["Attack1_Loop"];
        yield return new WaitForSeconds(time);
        monsterController.BoxCollider1.enabled = true;
        monsterController.PlaySound("NMS1_1");
        time = monsterController.clipDict["Attack1_Attack"];
        yield return new WaitForSeconds(time);
        monsterController.BoxCollider1.enabled = false;
        time = monsterController.clipDict["Attack1_End"];
        yield return new WaitForSeconds(time);
        StartCoroutine(monsterController.CoolDown1());
        attack = null;
        Exit();
    }
}
