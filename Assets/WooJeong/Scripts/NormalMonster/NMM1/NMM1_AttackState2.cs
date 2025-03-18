using EverScord.Character;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NMM1_AttackState2 : NAttackState
{
    NMM1_Controller controller;

    protected override void Setup()
    {
        monsterController = GetComponent<NMM1_Controller>();
        controller = monsterController as NMM1_Controller;
    }

    protected override IEnumerator Attack()
    {
        controller.ProjectLineIndicator(1f);
        yield return new WaitForSeconds(1f);
        monsterController.PlayAnimation("Attack2");
        float time = monsterController.clipDict["Attack2"];
        
        monsterController.PlaySound("NMM1_Sound");
        for (int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(time / 4 / 3);
            monsterController.BoxCollider2.enabled = true;
            yield return new WaitForSeconds(time / 4 / 3);
            monsterController.BoxCollider2.enabled = false;
            yield return new WaitForSeconds(time / 4 / 3);            
        }
        yield return new WaitForSeconds(time / 4);
        StartCoroutine(monsterController.CoolDown2());
        attack = null;
        Exit();
    }
}
