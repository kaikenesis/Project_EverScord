using System.Collections;
using UnityEngine;
using Photon.Pun;
using EverScord;

public class NMM2_AttackState2 : NAttackState
{
    private float damage;

    private void Start()
    {
        damage = monsterController.monsterData.Skill02_Damage;
    }

    protected override IEnumerator Attack()
    {
        monsterController.PlayAnimation("Attack2");
        float time = monsterController.clipDict["Attack2"];

        yield return new WaitForSeconds(time/2);
        monsterController.PlaySound("NMM2_2");
        monsterController.Fire("BossProjectile", damage);
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
