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
    private DecalProjector projector;
    private BoxCollider boxCollider;
    private float projectTime;
    GameObject effect;
    private string effectAddressableKey;
    ParticleSystem effectParticle;
    PhotonView photonView;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        projector = gameObject.AddComponent<DecalProjector>();
        projector.material = ResourceManager.Instance.GetAsset<Material>("DecalRedCircle");
        boxCollider = gameObject.AddComponent<BoxCollider>();
        //capCollider.isTrigger = true;
        projector.renderingLayerMask = 2;
    }

    public void Setup(float width, float projectTime, string effectAddressableKey, float attackDamage)
    {
        HP = MaxHP;
        this.projectTime = projectTime;
        this.effectAddressableKey = effectAddressableKey;
        this.attackDamage = attackDamage;
        projector.size = new Vector3(width, width, width);
        boxCollider.size = new Vector3(width, width, width);

        gameObject.transform.Rotate(90, 0, 0);
        projector.enabled = false;
        boxCollider.enabled = false;
        StartCoroutine(Attack());
    }

    public IEnumerator Attack()
    {
        projector.enabled = true;
        yield return new WaitForSeconds(projectTime);
        projector.enabled = false;

        boxCollider.enabled = true;
        effect = ResourceManager.Instance.GetFromPool(effectAddressableKey, transform.position, Quaternion.identity);
        effectParticle = effect.GetComponent<ParticleSystem>();
        yield return new WaitForSeconds(1.0f);
        effectParticle = effect.GetComponent<ParticleSystem>();
        effectParticle.Pause();
    }

    public void DecreaseHP(float hp)
    {
        photonView.RPC("SyncStoneHP", RpcTarget.All);
    }

    [PunRPC]
    private IEnumerator SyncStoneHP()
    {
        HP--;
        if (HP <= 0)
        {
            boxCollider.enabled = false;
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
}
