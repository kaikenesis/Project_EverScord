using DTT.AreaOfEffectRegions;
using EverScord.Character;
using EverScord.Effects;
using EverScord.Skill;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;

public class BossMonsterStoneAttack : MonoBehaviour, IEnemy
{
    private int HP = 30;
    private int MaxHP = 30;
    private float skillDamage;
    private float baseAttack;
    [SerializeField] GameObject circleProjectorObject;
    [SerializeField] SRPCircleRegionProjector circleProjector;
    private SphereCollider sphereCollider;
    private float projectTime;
    GameObject effect;
    private string effectAddressableKey;
    ParticleSystem effectParticle;
    PhotonView photonView;
    private BlinkEffect blinkEffect;    
    private CapsuleCollider capsuleCollider;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        sphereCollider = gameObject.AddComponent<SphereCollider>();
        capsuleCollider = gameObject.AddComponent<CapsuleCollider>();
        capsuleCollider.isTrigger = true;
    }

    public void Setup(float width, float projectTime, string effectAddressableKey, float skillDamage, float baseAttack)
    {
        HP = MaxHP;
        this.projectTime = projectTime;
        this.effectAddressableKey = effectAddressableKey;
        this.skillDamage = skillDamage;
        this.baseAttack = baseAttack;
        circleProjector.Radius = width / 2;
        sphereCollider.radius = width / 2;
        capsuleCollider.radius = width / 2;

        circleProjectorObject.SetActive(false);
        sphereCollider.enabled = false;
        capsuleCollider.enabled = false;

        StartCoroutine(Attack());
    }

    public IEnumerator Attack()
    {
        yield return StartCoroutine(ProjectCircle(1f));
        SoundManager.Instance.PlaySound("BossStoneUpSound");
        sphereCollider.enabled = true;
        capsuleCollider.enabled = true;
        effect = ResourceManager.Instance.GetFromPool(effectAddressableKey, transform.position, Quaternion.identity);
        effectParticle = effect.GetComponent<ParticleSystem>();

        if (blinkEffect == null)
            blinkEffect = BlinkEffect.Create(effectParticle);
        
        blinkEffect.InitParticles(effectParticle);

        yield return new WaitForSeconds(1.0f);
        effectParticle = effect.GetComponent<ParticleSystem>();
        effectParticle.Pause();
        capsuleCollider.enabled = false;
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
            SoundManager.Instance.PlaySound("BossPatternStoneBreak");
            sphereCollider.enabled = false;
            effectParticle.Play();
            yield return new WaitForSeconds(effectParticle.main.duration - 1f);
            ResourceManager.Instance.ReturnToPool(gameObject, "BossMonsterStoneAttack");
            ResourceManager.Instance.ReturnToPool(effect, effectAddressableKey);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            CharacterControl control = other.gameObject.GetComponent<CharacterControl>();
            float totalDamage = DamageCalculator.GetSkillDamage(baseAttack, skillDamage, 1, 1, control.Stats.Defense);
            control.DecreaseHP(totalDamage);
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

    public float GetDefense()
    {
        return 0;
    }

    public void SetDebuff(CharacterControl attacker, EBossDebuff debuffState, float time, float value)
    {
        throw new System.NotImplementedException();
    }

    public NavMeshAgent GetNavMeshAgent()
    {
        throw new System.NotImplementedException();
    }
}
