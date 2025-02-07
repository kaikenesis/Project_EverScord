using EverScord;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SK_121201_AttackState1 : NAttackState
{
    protected override void Setup()
    {
        monsterController = GetComponent<SK_121201_Controller>();
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
        foreach (var player in GameManager.Instance.playerPhotonViews)
        {
            if((player.transform.position - transform.position).magnitude <= monsterController.monsterData.AttackRangeX1
                && player.gameObject != monsterController.player)
            {
                player2 = player.gameObject;
                break;
            }
        }
        
        GameObject attackObj = new GameObject();
        //ResourceManager.Instance.LoadAsset("MonsterAttack", monsterController.player.transform.position);
        //GameObject attackObj = PhotonNetwork.Instantiate("MonsterAttack", monsterController.player.transform.position, Quaternion.identity);

        attackObj.name = "att1";
        attackObj.transform.position = monsterController.player.transform.position;
        MonsterAttack ma = attackObj.AddComponent<MonsterAttack>();
        ma.Setup(monsterController.monsterData.AttackRangeX1,
            monsterController.monsterData.ProjectionTime,
            monsterController.monsterData.DecalMat);

        if (player2 != null)
        {
            GameObject attackObj2 = new GameObject();
            attackObj2.name = "att1_2";
            attackObj2.transform.position = player2.transform.position;
            MonsterAttack ma2 = attackObj2.AddComponent<MonsterAttack>();
            ma2.Setup(monsterController.monsterData.AttackRangeX1,
                monsterController.monsterData.ProjectionTime,
                monsterController.monsterData.DecalMat);
        }
    }
}
