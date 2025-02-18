using EverScord.Character;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MonsterAttack : MonoBehaviour
{
    private float attackDamage;
    private DecalProjector projector;
    private CapsuleCollider capCollider;
    private float projectTime;
    private string effectAddressableKey;

    private void Awake()
    {
        projector = gameObject.AddComponent<DecalProjector>();
        projector.material = ResourceManager.Instance.GetAsset<Material>("DecalRedCircle");
        capCollider = gameObject.AddComponent<CapsuleCollider>();
        capCollider.isTrigger = true;
        projector.renderingLayerMask = 2;
    }

    public void Setup(float width, float projectTime, string effectAddressableKey, float attackDamage)
    {
        this.projectTime = projectTime;
        this.effectAddressableKey = effectAddressableKey;
        this.attackDamage = attackDamage;
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
        GameObject effect = ResourceManager.Instance.GetFromPool(effectAddressableKey, transform.position, Quaternion.identity);
        ParticleSystem effectParticle = effect.GetComponent<ParticleSystem>();
        effectParticle.Play();
        yield return new WaitForSeconds(effectParticle.main.duration);
        capCollider.enabled = false;
        ResourceManager.Instance.ReturnToPool(effect, effectAddressableKey);

        ResourceManager.Instance.ReturnToPool(gameObject, "MonsterAttack");
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            CharacterControl control = other.gameObject.GetComponent<CharacterControl>();
            control.DecreaseHP(attackDamage);
        }
    }
}
