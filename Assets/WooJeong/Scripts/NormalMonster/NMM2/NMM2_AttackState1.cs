using EverScord.Monster;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NMM2_AttackState1 : NAttackState
{
    private CapsuleCollider capsuleCollider;

    protected override IEnumerator Attack()
    {
        yield return project = StartCoroutine(monsterController.ProjectAttackRange(1));

        monsterController.PlayAnimation("Attack1");
        float time = monsterController.clipDict["Attack1"];

        monsterController.BoxCollider1.enabled = true;
        yield return new WaitForSeconds(time);
        monsterController.BoxCollider1.enabled = false;
        StartCoroutine(monsterController.CoolDown1());
        attack = null;
        Exit();
    }

    protected override void Setup()
    {
        capsuleCollider = gameObject.AddComponent<CapsuleCollider>();

        capsuleCollider.center = new Vector3(0, transform.position.y,
                                  monsterController.monsterData.AttackRangeZ1 / 2);

        capsuleCollider.radius = monsterController.monsterData.AttackRangeZ1;
        monsterController = GetComponent<NMM2_Controller>();
    }
}
