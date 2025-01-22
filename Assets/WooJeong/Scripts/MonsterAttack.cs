using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MonsterAttack : MonoBehaviour
{
    private DecalProjector projector;
    private CapsuleCollider capCollider;
    private float radius;
    private float projectTime;

    public MonsterAttack(float radius, float projectTime)
    {
        this.radius = radius;
        this.projectTime = projectTime;
        projector = gameObject.AddComponent<DecalProjector>();
        capCollider = gameObject.AddComponent<CapsuleCollider>();
        projector.size = new Vector3(radius, radius, radius);
        capCollider.radius = radius;

        projector.enabled = false;
        capCollider.enabled = false;
    }

    public IEnumerator Attack()
    {
        projector.enabled = true;
        yield return new WaitForSeconds(projectTime);
        projector.enabled = false;
        capCollider.enabled = true;
        yield return new WaitForSeconds(projectTime);
        capCollider.enabled = false;

        Destroy(gameObject);
    }
}
