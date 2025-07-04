using EverScord;
using EverScord.Character;
using EverScord.Skill;
using Photon.Pun;
using System.Collections;
using UnityEngine;

public class MonsterProjectile : MonoBehaviour
{
    public string ProjectileName { get; private set; }
    public GameObject ProjectileEffect { get; private set; }
    public int ID { get; private set; }
    private Vector3 direction;
    private float speed;
    private int lifeTime = 2;
    private float curTime = 0;
    private float baseDamage;
    private float skillDamage;
    private Coroutine move;
    private bool isMaxHPDamage = false;

    public bool IsDestroyed {  get; private set; }

    private void Awake()
    {
        ID = -1;
        IsDestroyed = false;
    }

    public void SetIsDestroyed(bool isDestroyed)
    {
        this.IsDestroyed = isDestroyed;
    }

    public void Setup(string projectileName, int id, Vector3 position, Vector3 direction, 
        float baseAttack, float skillDamage, float speed, bool isMaxHPDamage = false)
    {
        this.ProjectileName = projectileName;
        ID = id;
        this.baseDamage = baseAttack;
        this.skillDamage = skillDamage;
        IsDestroyed = false;
        transform.position = position;
        transform.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
        this.direction = direction;
        this.speed = speed;
        curTime = 0;
        this.isMaxHPDamage = isMaxHPDamage;
        move = StartCoroutine(Move());
    }

    private IEnumerator Move()
    {
        ProjectileEffect = ResourceManager.Instance.GetFromPool(ProjectileName, transform.position, Quaternion.identity);
        ProjectileEffect.transform.parent = transform;
        while (curTime < lifeTime)
        {
            transform.Translate(direction * speed * Time.deltaTime);
            curTime += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        curTime = 0;
        ResourceManager.Instance.ReturnToPool(ProjectileEffect, ProjectileName);
        ResourceManager.Instance.ReturnToPool(gameObject, "MonsterProjectile");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        if(other.gameObject.CompareTag("Player"))
        {
            CharacterControl controller = other.GetComponent<CharacterControl>();

            float totalDamage;
            if (isMaxHPDamage == true)
                totalDamage = skillDamage;
            else
                totalDamage = DamageCalculator.GetSkillDamage(baseDamage, skillDamage, 0, 0, controller.Stats.Defense);

            controller.DecreaseHP(totalDamage, isMaxHPDamage);
            StopCoroutine(move);
            IsDestroyed = true;
        }
        //else if (other.gameObject.CompareTag("Wall"))
        //{
        //    StopCoroutine(move);
        //    IsDestroyed = true;
        //}
        
    }
}
