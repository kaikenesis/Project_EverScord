using EverScord.Monster;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MonsterAttack : MonoBehaviour
{
    private DecalProjector projector;
    private CapsuleCollider capCollider;
    private float projectTime;

    private void Awake()
    {
        projector = gameObject.AddComponent<DecalProjector>();
        projector.material = ResourceManager.Instance.GetAsset<Material>("DecalRedCircle");
        capCollider = gameObject.AddComponent<CapsuleCollider>();
        projector.renderingLayerMask = 2;
    }

    public void Setup(float radius, float projectTime)
    {
        this.projectTime = projectTime;
        
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
