using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SK_121201_AttackState1 : NAttackState
{
    private GameObject player;

    protected override void Setup()
    {
        monsterController = GetComponent<SK_121201_Controller>();
    }
    protected override IEnumerator Attack()
    {
        Fire();
        monsterController.Animator.CrossFade("Attack1", 0.3f, -1, 0);

        float time = monsterController.clipDict["Attack1"]
                    + monsterController.clipDict["Attack1_Loop"]
                    + monsterController.clipDict["Attack1_End"];
        yield return new WaitForSeconds(time);

        StartCoroutine(monsterController.CoolDown1());
        attack = null;
        Exit();
    }

    private void Fire()
    {
        GameObject attackObj = new GameObject();
        attackObj.transform.position = monsterController.player.transform.position;
        MonsterAttack ma = attackObj.AddComponent<MonsterAttack>();
        ma.Setup(monsterController.monsterData.AttackRangeX1,
            monsterController.monsterData.ProjectionTime,
            monsterController.monsterData.DecalMat);
    }
}
