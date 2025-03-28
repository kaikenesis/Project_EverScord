using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NMS2_AttackState2 : NAttackState
{
    NMS2_Controller controller;

    protected override IEnumerator Attack()
    {        
        monsterController.PlayAnimation("Attack2");
        monsterController.PlaySound("NMS2_2");
        float time = monsterController.ClipDict["Attack2"];
        controller.isUpgraded = true;
        yield return new WaitForSeconds(time);
        StartCoroutine(monsterController.CoolDown2());
        attack = null;
        Exit();
    }

    protected override void Setup()
    {
        monsterController = GetComponent<NMS2_Controller>();
        controller = monsterController as NMS2_Controller;

    }
}
