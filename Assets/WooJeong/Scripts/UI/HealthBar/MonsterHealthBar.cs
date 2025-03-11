using DG.Tweening;
using EverScord.GameCamera;
using EverScord.Pool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterHealthBar : MonoBehaviour
{
    [SerializeField] Image healthBarFill;
    private Transform target;
    private Vector3 offset = new Vector3(0, 2f, 0);
    private float curHP = 0;
    private float maxHP = 0;

    private void Awake()
    {
        transform.rotation = CharacterCamera.CurrentClientCam.transform.rotation;
    }

    private void LateUpdate()
    {
        if (!target) return;
        transform.position = target.position + offset;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void SetOffsetY(float y)
    {
        offset.y = y;
    }

    public void UpdateHealth(float damage)
    {
        curHP -= damage;
        if (curHP < 0)
            curHP = 0;
        healthBarFill.DOFillAmount(curHP / maxHP, 0.5f);
        GameObject damageObj = ResourceManager.Instance.GetFromPool("DamageText", transform.position, Quaternion.identity);
        damageObj.transform.SetParent(transform);
        DamageTextUI damageUI = damageObj.GetComponent<DamageTextUI>();        
        damageUI.DisplayDamage(transform, damage);
    }

    public void InitHealthBar(float maxHealth)
    {
        maxHP = maxHealth;
        curHP = maxHealth;
        healthBarFill.DOFillAmount(curHP / maxHP, 0.5f);
    }
}
