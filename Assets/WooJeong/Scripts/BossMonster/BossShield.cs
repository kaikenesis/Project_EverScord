using System;
using EverScord.Character;
using EverScord.Effects;
using UnityEngine;
using UnityEngine.AI;

public class BossShield : MonoBehaviour, IEnemy
{
    public float HP {  get; private set; }

    public BodyType EnemyBodyType => BodyType.BARRIER;

    private BlinkEffect blinkEffect;

    void Awake()
    {
        var particle = GetComponent<ParticleSystem>();
        blinkEffect = BlinkEffect.Create(particle);
        blinkEffect.InitParticles(particle);
    }

    public void SetHP(float hp)
    {
        HP = hp;
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

    public float GetDefense()
    {
        return 0;
    }

    public void SetDebuff(CharacterControl attacker, EBossDebuff debuffState, float time, float value)
    {
        return;
    }

    public NavMeshAgent GetNavMeshAgent()
    {
        throw new NotImplementedException();
    }
}
