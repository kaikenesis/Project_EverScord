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

    private void LateUpdate()
    {
        if (!target) return;
        transform.position = target.position + offset;
        if (Camera.main != null)
            transform.rotation = Camera.main.transform.rotation;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void SetOffsetY(float y)
    {
        offset.y = y;
    }

    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        healthBarFill.fillAmount = currentHealth / maxHealth;
    }
}
