using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SK_121201_AttackState2 : NAttackState
{
    //private GameObject player;

    protected override void Setup()
    {
        monsterController = GetComponent<SK_121201_Controller>();
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
        GameObject attackObj = new GameObject();
        attackObj.transform.position = monsterController.player.transform.position;
        MonsterAttack ma = attackObj.AddComponent<MonsterAttack>();
        ma.Setup(monsterController.monsterData.AttackRangeX2,
            monsterController.monsterData.ProjectionTime,
            monsterController.monsterData.DecalMat);
    }
}
