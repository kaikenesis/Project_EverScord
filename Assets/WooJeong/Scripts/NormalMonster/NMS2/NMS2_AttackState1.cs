using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NMS2_AttackState1 : NAttackState
{
    NMS2_Controller controller;
    private float damage;

    private void Start()
    {
        damage = monsterController.monsterData.Skill01_Damage;
    }

    protected override IEnumerator Attack()
    {
        monsterController.PlayAnimation("Attack1");
        float time = monsterController.clipDict["Attack1"];
        int attackCount = 5;
        if (controller.isUpgraded)
        {
            attackCount = 8;
            controller.isUpgraded = false;
        }
        yield return new WaitForSeconds(1f);
        time -= 4f;

        for (int i = 0; i < attackCount; i++)
        {
            monsterController.PlaySound("NMS2_1");
            monsterController.Fire("NMM2_Projectile", damage);
            yield return new WaitForSeconds(time / attackCount);
        }
        yield return new WaitForSeconds(3);
        StartCoroutine(monsterController.CoolDown1());
        attack = null;
        Exit();
    }

    protected override void Setup()
    {
        monsterController = GetComponent<NMS2_Controller>();
        controller = monsterController as NMS2_Controller;
    }
}
