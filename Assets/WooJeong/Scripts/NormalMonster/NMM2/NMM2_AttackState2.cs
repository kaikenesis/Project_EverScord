using System.Collections;
using UnityEngine;
using Photon.Pun;
using EverScord;

public class NMM2_AttackState2 : NAttackState
{
    protected override IEnumerator Attack()
    {
        monsterController.PlayAnimation("Attack2");
        float time = monsterController.clipDict["Attack2"];
        yield return new WaitForSeconds(time/2);
        monsterController.Fire("BossProjectile");
        yield return new WaitForSeconds(time/2);

        StartCoroutine(monsterController.CoolDown2());
        attack = null;
        Exit();
    }

    protected override void Setup()
    {
        monsterController = GetComponent<NMM2_Controller>();
    }
}
