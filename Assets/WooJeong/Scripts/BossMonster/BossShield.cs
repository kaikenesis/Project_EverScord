using System;
using EverScord.Character;
using EverScord.Effects;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class BossShield : MonoBehaviour, IEnemy
{
    public float HP {  get; private set; }
    private float maxHP = 100;
    private BlinkEffect blinkEffect;

    void Awake()
    {
        var particle = GetComponent<ParticleSystem>();
        blinkEffect = BlinkEffect.Create(particle);
        blinkEffect.InitParticles(particle);
    }

    private void OnEnable()
    {
        HP = maxHP;
    }

    public void DecreaseHP(float hp, CharacterControl attacker)
    {
        HP -= hp;
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
