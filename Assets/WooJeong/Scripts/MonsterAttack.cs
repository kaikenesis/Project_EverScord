using DTT.AreaOfEffectRegions;
using EverScord.Character;
using EverScord.Skill;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MonsterAttack : MonoBehaviour
{
    private float baseAttack;
    private float skillDamage;
    [SerializeField] GameObject circleProjectorObject;
    [SerializeField] SRPCircleRegionProjector circleProjector;
    private CapsuleCollider capCollider;
    private float projectTime;
    private string effectAddressableKey;

    private void Awake()
    {
        capCollider = gameObject.AddComponent<CapsuleCollider>();
        capCollider.isTrigger = true;
    }

    public void Setup(float width, float projectTime, string effectAddressableKey, float baseAttack, float skillDamage)
    {
        this.projectTime = projectTime;
        this.effectAddressableKey = effectAddressableKey;
        this.baseAttack = baseAttack;
        this.skillDamage = skillDamage;
        circleProjector.Radius = width;
        capCollider.radius = width/2;

        circleProjectorObject.SetActive(false);
        capCollider.enabled = false;

        StartCoroutine(Attack());
    }

    public IEnumerator Attack()
    {
        yield return StartCoroutine(ProjectCircle(projectTime));

        capCollider.enabled = true;
        GameObject effect = ResourceManager.Instance.GetFromPool(effectAddressableKey, transform.position, Quaternion.identity);
        ParticleSystem effectParticle = effect.GetComponent<ParticleSystem>();
        effectParticle.Play();
        yield return new WaitForSeconds(effectParticle.main.duration);
        capCollider.enabled = false;
        ResourceManager.Instance.ReturnToPool(effect, effectAddressableKey);

        ResourceManager.Instance.ReturnToPool(gameObject, "MonsterAttack");
    }

    private IEnumerator ProjectCircle(float duration)
    {
        circleProjectorObject.SetActive(true);
        circleProjector.FillProgress = 0;
        float t = 0f;
        while (true)
        {
            t += Time.deltaTime;
            if (t >= duration)
            {
                circleProjectorObject.SetActive(false);
                yield break;
            }
            circleProjector.FillProgress = t / duration;
            circleProjector.UpdateProjectors();
            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            CharacterControl control = other.gameObject.GetComponent<CharacterControl>();
            float totalDamage = DamageCalculator.GetSkillDamage(baseAttack, skillDamage, 0, 0, control.Defense);
            control.DecreaseHP(totalDamage);
        }
    }
}
