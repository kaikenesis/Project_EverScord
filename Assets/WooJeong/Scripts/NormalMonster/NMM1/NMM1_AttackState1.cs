using EverScord.Character;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class NMM1_AttackState1 : NAttackState
{
    protected override void Setup()
    {
        monsterController = GetComponent<NMM1_Controller>();
    }

    protected override IEnumerator Attack()
    {
        yield return project = StartCoroutine(monsterController.ProjectAttackRange(1));

        monsterController.PlayAnimation("Attack1");
        float time = monsterController.clipDict["Attack1"];

        yield return new WaitForSeconds(time / 3);
        monsterController.BoxCollider1.enabled = true;
        yield return new WaitForSeconds(time / 3);
        monsterController.BoxCollider1.enabled = false;
        yield return new WaitForSeconds(time / 3);
        StartCoroutine(monsterController.CoolDown1());
        attack = null;
        Exit();
    }
}
