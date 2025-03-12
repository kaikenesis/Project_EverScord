using DTT.AreaOfEffectRegions;
using EverScord.Character;
using EverScord.Effects;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BossMonsterStoneAttack : MonoBehaviour, IEnemy
{
    private int HP = 30;
    private int MaxHP = 30;
    private float attackDamage;
    [SerializeField] GameObject circleProjectorObject;
    [SerializeField] SRPCircleRegionProjector circleProjector;
    private SphereCollider sphereCollider;
    private float projectTime;
    GameObject effect;
    private string effectAddressableKey;
    ParticleSystem effectParticle;
    PhotonView photonView;
    private BlinkEffect blinkEffect;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        sphereCollider = gameObject.AddComponent<SphereCollider>();
    }

    public void Setup(float width, float projectTime, string effectAddressableKey, float attackDamage)
    {
        HP = MaxHP;
        this.projectTime = projectTime;
        this.effectAddressableKey = effectAddressableKey;
        this.attackDamage = attackDamage;
        circleProjector.Radius = width / 2;
        sphereCollider.radius = width / 2;

        circleProjectorObject.SetActive(false);
        sphereCollider.enabled = false;
        StartCoroutine(Attack());
    }

    public IEnumerator Attack()
    {
        yield return StartCoroutine(ProjectCircle(1f));

        sphereCollider.enabled = true;
        effect = ResourceManager.Instance.GetFromPool(effectAddressableKey, transform.position, Quaternion.identity);
        effectParticle = effect.GetComponent<ParticleSystem>();

        if (blinkEffect == null)
            blinkEffect = BlinkEffect.Create(effectParticle);
        
        blinkEffect.InitParticles(effectParticle);

        yield return new WaitForSeconds(1.0f);
        effectParticle = effect.GetComponent<ParticleSystem>();
        effectParticle.Pause();
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

    public void DecreaseHP(float hp, CharacterControl attacker)
    {
        photonView.RPC("SyncStoneHP", RpcTarget.All);
    }

    [PunRPC]
    private IEnumerator SyncStoneHP()
    {
        HP--;
        if (HP <= 0)
        {
            sphereCollider.enabled = false;
            effectParticle.Play();
            yield return new WaitForSeconds(effectParticle.main.duration - 1f);
            ResourceManager.Instance.ReturnToPool(gameObject, "BossMonsterStoneAttack");
            ResourceManager.Instance.ReturnToPool(effect, effectAddressableKey);
        }
    }

    public void StunMonster(float stunTime)
    {
        return;
    }

    public void TestDamage(GameObject sender, float value)
    {
        throw new System.NotImplementedException();
    }

    public BlinkEffect GetBlinkEffect()
    {
        return blinkEffect;
    }
}
