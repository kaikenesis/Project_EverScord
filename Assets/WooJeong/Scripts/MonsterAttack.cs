using EverScord.Monster;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MonsterAttack : MonoBehaviour
{
    private DecalProjector projector;
    private CapsuleCollider capCollider;
    private Material decalMat;
    private float radius;
    private float projectTime;
    private void Awake()
    {
        projector = gameObject.AddComponent<DecalProjector>();
        capCollider = gameObject.AddComponent<CapsuleCollider>();

    }

    public void Setup(float radius, float projectTime, Material decalMat)
    {
        this.radius = radius;
        this.projectTime = projectTime;
        this.decalMat = decalMat;
        
        projector.material = decalMat;
        projector.size = new Vector3(radius * 2, radius * 2, radius * 2);
        capCollider.radius = radius;

        gameObject.transform.Rotate(90, 0, 0);
        projector.enabled = false;
        capCollider.enabled = false;

        StartCoroutine(Attack());
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
