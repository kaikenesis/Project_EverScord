using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BossPattern04_Imp : AttackNodeImplement
{
    private float chargeRange = 10;
    private BoxCollider boxCollider;

    protected override void Awake()
    {
        base.Awake();
        attackableHP = 90;
        boxCollider = transform.AddComponent<BoxCollider>();
        boxCollider.size = new Vector3(2, 1, 3);
        boxCollider.center = new Vector3(0, 1, 1.5f);
        boxCollider.isTrigger = true;
        boxCollider.enabled = false;
    }

    protected override IEnumerator Act()
    {
        bossRPC.PlayAnimation("Idle");
        yield return bossRPC.ProjectEnable(4, 1f);

        bossRPC.PlayAnimation("RushAttack");
        StartCoroutine(Charge(0.5f));
        yield return new WaitForSeconds(0.7f);
        bossRPC.PlayEffect("StandingAttackEffect", transform.position + transform.forward * 5);
        yield return new WaitForSeconds(2f);
        bossRPC.PlayAnimation("Idle");
        isEnd = true;
        action = null;
    }

    private IEnumerator Charge(float duration)
    {
        boxCollider.enabled = true;
        Vector3 startPoint = transform.position;
        Vector3 endPoint = transform.position + transform.forward * (chargeRange-3);
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            transform.position = Vector3.Lerp(startPoint, endPoint, t / duration);
            yield return new WaitForSeconds(Time.deltaTime);
        }
        boxCollider.enabled = false;
    }
}
