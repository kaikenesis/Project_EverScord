using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NML2_AttackState2 : NAttackState
{
    protected override void Setup()
    {
        monsterController = GetComponent<NML2_Controller>();
    }
    protected override IEnumerator Attack()
    {
        float time = monsterController.clipDict["Attack2"];
        //monsterController.Animator.CrossFade("Attack2", 0.3f, -1, 0);
        monsterController.PlayAnimation("Attack2");

        for (int i = 0; i < 3; i++)
        {
            Fire();
            yield return new WaitForSeconds(2f);
        }

        yield return new WaitForSeconds(time - 6f);

        StartCoroutine(monsterController.CoolDown2());
        attack = null;
        Exit();
    }

    private void Fire()
    {
        monsterController.InstantiateMonsterAttack(
            monsterController.player.transform.position,
            monsterController.monsterData.AttackRangeX2,
            monsterController.monsterData.ProjectionTime,
            "NML2_A2_Effect",
            monsterController.monsterData.AttackDamage2);
    }
}
