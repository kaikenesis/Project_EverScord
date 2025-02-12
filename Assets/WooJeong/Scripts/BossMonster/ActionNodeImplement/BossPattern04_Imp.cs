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
        boxCollider = GetComponent<BoxCollider>();
        projector = GetComponent<DecalProjector>();
        projector.enabled = false;
        projector.size = new Vector3(1, 1, 10);
        projector.pivot = new Vector3(0.5f, 0f, 5f);
        boxCollider.enabled = false;
        boxCollider.size = new Vector3(1, 1, 1);
    }

    protected override IEnumerator Action()
    {
        projector.size = new Vector3(1,1,10);
        projector.pivot = new Vector3(0.5f, 0f, 5f);
        projector.enabled = true;
        yield return new WaitForSeconds(2f);
        projector.enabled = false;
        bossRPC.PlayAnimation("RushAttack");
        yield return StartCoroutine(Charge(1));
        
        yield return new WaitForSeconds(2f);
    }

    private IEnumerator Charge(float duration)
    {
        Vector3 startPoint = transform.position;
        Vector3 endPoint = transform.position + transform.forward * (chargeRange);
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            transform.position = Vector3.Lerp(startPoint, endPoint, t / duration);
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }
}
