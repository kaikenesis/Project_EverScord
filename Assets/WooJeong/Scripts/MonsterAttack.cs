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
    private string addressableKey;

    private void Awake()
    {
        projector = gameObject.AddComponent<DecalProjector>();
        projector.material = ResourceManager.Instance.GetAsset<Material>("DecalRedCircle");
        capCollider = gameObject.AddComponent<CapsuleCollider>();
        projector.renderingLayerMask = 2;
    }

    public void Setup(float width, float projectTime, string addressableKey)
    {
        this.projectTime = projectTime;
        this.addressableKey = addressableKey;
        projector.size = new Vector3(width, width, width);
        capCollider.radius = width/2;

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
        GameObject effect = ResourceManager.Instance.GetFromPool(addressableKey, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(projectTime);
        capCollider.enabled = false;
        ResourceManager.Instance.ReturnToPool(effect, addressableKey);

        Destroy(gameObject);
    }
}
