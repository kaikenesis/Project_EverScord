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
        monsterController.PlayAnimation("Attack2");
        monsterController.PlaySound("NML2_2");
        
        for (int i = 0; i < 3; i++)
        {
            Fire();
            yield return new WaitForSeconds(1.5f);
        }

        yield return new WaitForSeconds(time - 4.5f);

        StartCoroutine(monsterController.CoolDown2());
        attack = null;
        Exit();
    }

    private void Fire()
    {
        monsterController.InstantiateMonsterAttack(
            monsterController.player.transform.position,
            monsterController.monsterData.Skill02_RangeX,
            monsterController.monsterData.ProjectionTime,
            "NML2_A2_Effect",
            monsterController.monsterData.Skill02_Damage,
            "NML2_2_Lightning");
    }
}
