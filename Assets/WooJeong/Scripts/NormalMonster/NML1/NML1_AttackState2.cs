using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class NML1_AttackState2 : NAttackState
{
    private Vector3 moveVector;
    private Vector3 startVector;
    private float chargeRange;
    private NML1_Controller controller;    

    protected override void Setup()
    {
        monsterController = GetComponent<NML1_Controller>();
        controller = monsterController as NML1_Controller;
        var temp = controller.monsterData as NML1_Data;
        chargeRange = temp.ChargeRange;
    }

    protected override IEnumerator Attack()
    {
        startVector = transform.position;
        moveVector = (monsterController.player.transform.position - transform.position).normalized;
        
        controller.PhotonView.RPC(nameof(SyncProjectLineIndicator), RpcTarget.All, 1.0f);
        yield return new WaitForSeconds(1f);
        monsterController.PlayAnimation("Attack2");
        float time = monsterController.clipDict["Attack2"];

        yield return new WaitForSeconds(time / 4);
        monsterController.PlaySound("NML1_2");
        monsterController.BoxCollider2.enabled = true;
        StartCoroutine(Charge(1f));
        yield return new WaitForSeconds(time / 4 * 3);
        monsterController.BoxCollider2.enabled = false;
        StartCoroutine(monsterController.CoolDown2());
        attack = null;
        Exit();
    }

    private IEnumerator Charge(float duration)
    {
        Vector3 endPoint = startVector + moveVector * (chargeRange - monsterController.monsterData.Skill02_RangeZ);
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            transform.position = Vector3.Lerp(startVector, endPoint, t / duration);
            yield return null;
        }
    }

    [PunRPC]
    private IEnumerator SyncProjectLineIndicator(float duration)
    {
        controller.LineIndicator.SetActive(true);
        controller.LineProjector.FillProgress = 0;
        float t = 0f;
        while (true) 
        {
            t += Time.deltaTime;
            if(t >= duration)
            {
                controller.LineIndicator.SetActive(false);
                yield break;
            }
            controller.LineProjector.FillProgress = t / duration;
            controller.LineProjector.UpdateProjectors();
            yield return null;
        }
    }
}
