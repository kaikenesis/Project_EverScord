using EverScord;
using System.Collections;
using EverScord.Character;
using UnityEngine;

public class NML2_AttackState1 : NAttackState
{
    protected override void Setup()
    {
        monsterController = GetComponent<NML2_Controller>();
    }
    protected override IEnumerator Attack()
    {
        Fire();
        //monsterController.Animator.CrossFade("Attack1", 0.3f, -1, 0);
        monsterController.PlayAnimation("Attack1");

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
        GameObject player2 = null;
        foreach (CharacterControl player in GameManager.Instance.PlayerDict.Values)
        {
            if((player.PlayerTransform.position - transform.position).magnitude <= monsterController.monsterData.AttackRangeX1
                && player.gameObject != monsterController.player)
            {
                player2 = player.gameObject;
                break;
            }
        }

        monsterController.InstantiateMonsterAttack(
            monsterController.player.transform.position,
            monsterController.monsterData.AttackRangeX1,
            monsterController.monsterData.ProjectionTime,
            "NML2_A1_Effect01",
            monsterController.monsterData.AttackDamage1);

        if (player2 != null)
        {
            monsterController.InstantiateMonsterAttack(
                player2.transform.position,
                monsterController.monsterData.AttackRangeX1,
                monsterController.monsterData.ProjectionTime,
                "NML2_A1_Effect01",
                monsterController.monsterData.AttackDamage1);
        }
    }
}
