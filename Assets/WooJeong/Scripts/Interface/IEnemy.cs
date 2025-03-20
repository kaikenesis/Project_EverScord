using UnityEngine;
using EverScord.Effects;
using EverScord.Character;
using UnityEngine.AI;

public interface IEnemy
{
    public void TestDamage(GameObject sender, float value);
    public void DecreaseHP(float hp, CharacterControl attacker);
    public void StunMonster(float stunTime);
    public BlinkEffect GetBlinkEffect();
    public BodyType EnemyBodyType { get; }

    public float GetDefense();

    public void SetDebuff(CharacterControl attacker, EBossDebuff debuffState, float time, float value);
    public NavMeshAgent GetNavMeshAgent();

}
