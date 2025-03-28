using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NMS1_AttackState2 : NAttackState
{
    protected override void Setup()
    {
        monsterController = GetComponent<NMS1_Controller>();
    }
    
    protected override IEnumerator Attack()
    {
        monsterController.PlayAnimation("Attack2");
        monsterController.PlaySound("NMS1_2");
        float time = monsterController.ClipDict["Attack2"];
        monsterController.BoxCollider2.enabled = true;
        yield return new WaitForSeconds(time);
        monsterController.BoxCollider2.enabled = false;
        StartCoroutine(monsterController.CoolDown2());
        attack = null;
        Exit();
    }
}
