using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BossShield : MonoBehaviour, IEnemy
{
    public float HP {  get; private set; }
    private float maxHP = 100;

    private void OnEnable()
    {
        HP = maxHP;
    }

    public void DecreaseHP(float hp)
    {
        HP -= hp;
    }

    public void StunMonster(float stunTime)
    {
        return;
    }
}
