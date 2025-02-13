using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BossPattern04_Imp : ActionNodeImplement
{
    private float chargeRange = 10;
    private BoxCollider boxCollider;
    private DecalProjector projector;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override IEnumerator Act()
    {
        Debug.Log("Attack4 start");
        bossRPC.PlayAnimation("Idle");
        yield return bossRPC.ChargeProjectEnable(1f);

        bossRPC.PlayAnimation("RushAttack");
        yield return StartCoroutine(Charge(1));
        
        yield return new WaitForSeconds(1.3f);
        bossRPC.PlayAnimation("Idle");
        isEnd = true;
        action = null;
        Debug.Log("Attack4 end");
    }

    private IEnumerator Charge(float duration)
    {
        Vector3 startPoint = transform.position;
        Vector3 endPoint = transform.position + transform.forward * (chargeRange-3);
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            transform.position = Vector3.Lerp(startPoint, endPoint, t / duration);
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }
}
