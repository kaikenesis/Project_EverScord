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
        monsterController.PlayAnimation("Attack1");

        float time = monsterController.clipDict["Attack1"]
                    + monsterController.clipDict["Attack1_Loop"]
                    + monsterController.clipDict["Attack1_End"];
        monsterController.PlaySound("NML2_1");
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
            if((player.PlayerTransform.position - transform.position).magnitude <= monsterController.monsterData.Skill01_RangeX
                && player.gameObject != monsterController.player)
            {
                player2 = player.gameObject;
                break;
            }
        }

        monsterController.InstantiateMonsterAttack(
            monsterController.player.transform.position,
            monsterController.monsterData.Skill01_RangeX,
            monsterController.monsterData.ProjectionTime,
            "NML2_A1_Effect01",
            monsterController.monsterData.Skill01_Damage,
            "NML2_Floor");

        if (player2 != null)
        {
            monsterController.InstantiateMonsterAttack(
                player2.transform.position,
                monsterController.monsterData.Skill01_RangeX,
                monsterController.monsterData.ProjectionTime,
                "NML2_A1_Effect01",
                monsterController.monsterData.Skill01_Damage,
                "NML2_Floor");
        }
    }
}
